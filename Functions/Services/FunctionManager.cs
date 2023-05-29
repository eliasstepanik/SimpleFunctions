﻿using Functions.Data;
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
    
    public FunctionManager(ILogger<FunctionManager> logger, IDockerManager dockerManager, IDbContextFactory<FunctionsContext> dbContextFactory, IExternalEndpointManager externalEndpointManager)
    {
        _logger = logger;
        _dockerManager = dockerManager;
        _dbContextFactory = dbContextFactory;
        _externalEndpointManager = externalEndpointManager;
    }


    public async Task CreateFunction(string functionName, string imageTag)
    {
        var db = await _dbContextFactory.CreateDbContextAsync();
        await db.Functions.AddAsync(new Function(functionName, imageTag));
        await db.SaveChangesAsync();
    }
    public async Task DeleteFunction(string functionName)
    {
        var db = await _dbContextFactory.CreateDbContextAsync();
        var function = db.Functions.Include(s => s.Instances).Include(s => s.EnvironmentVariables).First(s => s.Name.Equals(functionName));
        foreach (var functionInstance in function.Instances)
        {
            _dockerManager.DeleteContainer(functionInstance.InstanceId);
        }
        db.Functions.Remove(function);
        await db.SaveChangesAsync();
    }

    public async Task<string> RunInstance(string functionName, HttpMethod method, string body = "")
    {
        var db = await _dbContextFactory.CreateDbContextAsync();
        var function = db.Functions.Include(s => s.Instances).Include(s => s.EnvironmentVariables).First(s => s.Name.Equals(functionName));
        
        var container = await _dockerManager.CreateContainer(function.ImageTag,function.EnvironmentVariables);
        var instance = new Instance(container.Name,container.Id);
        function.Instances.Add(instance);
        db.Functions.Update(function);
        await db.SaveChangesAsync();
        
        _dockerManager.ConnectNetwork("simplefunctions_functions", instance.InstanceId);
        _dockerManager.StartContainer(instance.InstanceId);
        
        //TODO: If not started delete instance
        //Send Request to Container

        if (method.Equals(HttpMethod.Post))
        {
            var message = await _externalEndpointManager.Post(instance.Name, body);
            if (message.Equals("error"))
            {
                _dockerManager.DeleteContainer(instance.InstanceId);
                return "";
            }

            return message;
        }
        if (method.Equals(HttpMethod.Get))
        {
            var message = await _externalEndpointManager.Get(instance.Name);
            if (message.Equals("error"))
            {
                _dockerManager.DeleteContainer(instance.InstanceId);
                return "";
            }

            return message;
        }
        if (method.Equals(HttpMethod.Patch))
        {
            var message = await _externalEndpointManager.Patch(instance.Name, body);
            if (message.Equals("error"))
            {
                _dockerManager.DeleteContainer(instance.InstanceId);
                return "";
            }

            return message;

        }

        return "";
    }
    
    
}