using System.Net;
using Functions.Data;
using Functions.Data.DB;
using Functions.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Functions.Services;

public class FunctionManager
{
    //TODO: Add Logging
    private readonly ILogger<FunctionManager> _logger;
    private readonly IDockerManager _dockerManager;
    private readonly IDbContextFactory<FunctionsContext> _dbContextFactory;
    private readonly IExternalEndpointManager _externalEndpointManager;
    private readonly IConfiguration _configuration;
    private readonly INativeCommandWrapper _nativeCommandWrapper;
    
    public FunctionManager(ILogger<FunctionManager> logger, IDockerManager dockerManager, IDbContextFactory<FunctionsContext> dbContextFactory, IExternalEndpointManager externalEndpointManager, IConfiguration configuration, INativeCommandWrapper nativeCommandWrapper)
    {
        _logger = logger;
        _dockerManager = dockerManager;
        _dbContextFactory = dbContextFactory;
        _externalEndpointManager = externalEndpointManager;
        _configuration = configuration;
        _nativeCommandWrapper = nativeCommandWrapper;
    }


    public async Task CreateFunction(string functionName, string imageTag)
    {
        var db = await _dbContextFactory.CreateDbContextAsync();
        await db.Functions.AddAsync(new Function(functionName, imageTag));
        await db.SaveChangesAsync();
        await db.DisposeAsync();
    }
    public async Task DeleteFunction(string functionName)
    {
        var db = await _dbContextFactory.CreateDbContextAsync();
        var function = db.Functions.Include(s => s.Instances).Include(s => s.EnvironmentVariables).First(s => s.Name.Equals(functionName));
        foreach (var functionInstance in function.Instances)
        {
            DeleteInstance(functionInstance);
        }
        db.Functions.Remove(function);
        await db.SaveChangesAsync();
        await db.DisposeAsync();
    }

    public async Task<InstanceRuntimeInfo> RunInstance(string functionName)
    {
        var db = await _dbContextFactory.CreateDbContextAsync();
        var function = db.Functions.Include(s => s.Instances).Include(s => s.EnvironmentVariables).First(s => s.Name.Equals(functionName));
        
        var container = await _dockerManager.CreateContainer(function.ImageTag,function.EnvironmentVariables);
        var instance = new Instance(container.Name,container.Id);
        function.Instances.Add(instance);
        db.Functions.Update(function);
        await db.SaveChangesAsync();
        await db.DisposeAsync();
        
        _dockerManager.ConnectNetwork(_configuration["AppConfig:FuctionNetworkName"] ?? throw new InvalidOperationException(), instance.InstanceId);
        _dockerManager.StartContainer(instance.InstanceId);


        return new InstanceRuntimeInfo()
        {
            Instance = instance
        };
    }

    public async Task<HttpResponseMessage> SendRequest(Instance? instance,string function, HttpMethod method, string body = "")
    {
        if (method.Equals(HttpMethod.Post))
        {
            var message = await _externalEndpointManager.Post(instance.Name, body);
            return await HandleError(message, instance);
        }
        if (method.Equals(HttpMethod.Get))
        {
            var message = await _externalEndpointManager.Get(instance.Name);
            return await HandleError(message, instance);
        }
        if (method.Equals(HttpMethod.Patch))
        {
            var message = await _externalEndpointManager.Patch(instance.Name, body);
            return await HandleError(message, instance);


        }
        if (method.Equals(HttpMethod.Put))
        {
            var message = await _externalEndpointManager.Put(instance.Name, body);
            return await HandleError(message, instance);
        }
        if (method.Equals(HttpMethod.Delete))
        {
            var message = await _externalEndpointManager.Delete(instance.Name);
            return await HandleError(message, instance);
        }
        return new HttpResponseMessage(HttpStatusCode.BadRequest);
    }


    private async Task<HttpResponseMessage> HandleError(HttpResponseMessage message, Instance? instance)
    {
        var db = await _dbContextFactory.CreateDbContextAsync();
        if (!message.IsSuccessStatusCode)
        {
            DeleteInstance(instance);
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
        return message;
    }

    public async void DeleteInstance(Instance? instance)
    {
        var db = await _dbContextFactory.CreateDbContextAsync();
        _dockerManager.DeleteContainer(instance.InstanceId);
        var temp_i = db.Instances.First(s => s.InstanceId.Equals(instance.InstanceId));
        db.Instances.Remove(temp_i);
        await db.SaveChangesAsync();
        await db.DisposeAsync();
    }


}