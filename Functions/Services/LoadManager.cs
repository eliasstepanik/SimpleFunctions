using Functions.Data;
using Functions.Data.DB;
using Functions.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Functions.Services;

public class LoadManager : ILoadManager
{
    private readonly FunctionManager _functionManager;
    private readonly IDbContextFactory<FunctionsContext> _dbContextFactory;
    private readonly IDockerManager _dockerManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LoadManager> _logger;

    public LoadManager(FunctionManager functionManager, IDbContextFactory<FunctionsContext> dbContextFactory, IDockerManager dockerManager,IServiceProvider serviceProvider, ILogger<LoadManager> logger)
    {
        _functionManager = functionManager;
        _dbContextFactory = dbContextFactory;
        _dockerManager = dockerManager;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<HttpResponseMessage> HandleRequest(string functionName, HttpMethod method, string body = "")
    {
        var db = await _dbContextFactory.CreateDbContextAsync();
        var function = await db.Functions.Include(s => s.Instances).FirstAsync(x => x.Name.Equals(functionName));
        if (function.Instances.Count == 0)
        {
            await _functionManager.RunInstance(function.Name);
        }
        foreach (var dbFunction in db.Functions.Include(s => s.Instances))
        {
            if (dbFunction.Instances.Count == 0)
            {
                await _functionManager.RunInstance(dbFunction.Name);
            }
        }

        var instance = await GetLowestLoadInstance(functionName);
        var responseMessage = await _functionManager.SendRequest(instance, functionName, method, body);
        return responseMessage;
    }
    
    private async Task<bool> IsOverloaded(string containerId)
    {
        var load = await _dockerManager.GetLoad(containerId);
        if (JsonConvert.SerializeObject(load).Equals("{}"))
        {
            return false;
        }
        try
        {
            var usedMemory = load.MemoryStats.Usage;
            var availableMemory = load.MemoryStats.Limit;
            // ReSharper disable once PossibleLossOfFraction
            var memoryUsage = (usedMemory / availableMemory) * 100.0;
            var cpuDelta = load.CPUStats.CPUUsage.TotalUsage - load.PreCPUStats.CPUUsage.TotalUsage;
            var systemCpuDelta = load.CPUStats.SystemUsage - load.PreCPUStats.CPUUsage.TotalUsage;
            var numberCpus = load.CPUStats.OnlineCPUs;
            // ReSharper disable once PossibleLossOfFraction
            var cpuUsage = (cpuDelta / systemCpuDelta) * numberCpus * 100.0;
            if (cpuUsage > 80)
            {
                return true;
            }
            if(memoryUsage > 80)
            {
                return true;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
        

        return false;
    }

    private async Task<Instance?> GetLowestLoadInstance(string functionName)
    {
        Instance? lowestUsageCpuInstance = null;
        double lowestUsageCpu = double.MaxValue;
        
        Instance? lowestUsageMemoryInstance = null;
        double lowestUsageMemory = double.MaxValue;
        var db = await _dbContextFactory.CreateDbContextAsync();
        try
        {
            
            foreach (var function in db.Functions.Include(s => s.Instances))
            {
                foreach (var functionInstance in function.Instances)
                {
                    var load = await _dockerManager.GetLoad(functionInstance.InstanceId);
                    var usedMemory = load.MemoryStats.Usage;
                    var availableMemory = load.MemoryStats.Limit;
                    var cpuDelta = load.CPUStats.CPUUsage.TotalUsage - load.PreCPUStats.CPUUsage.TotalUsage;
                    var systemCpuDelta = load.CPUStats.SystemUsage - load.PreCPUStats.CPUUsage.TotalUsage;
                    var numberCpus = load.CPUStats.OnlineCPUs;
                    // ReSharper disable once PossibleLossOfFraction
                
                    //TODO: Later
                    var cpuUsage = (cpuDelta / systemCpuDelta) * numberCpus * 100.0;
                    // ReSharper disable once PossibleLossOfFraction
                    var memoryUsage = (usedMemory / availableMemory) * 100.0;

                    if (cpuUsage <= lowestUsageCpu)
                    {
                        lowestUsageCpu = cpuUsage;
                        lowestUsageCpuInstance = functionInstance;
                    }

                    if (memoryUsage <= lowestUsageMemory)
                    {
                        lowestUsageMemory = memoryUsage;
                        lowestUsageMemoryInstance = functionInstance;
                    }
                
                }
            }
            
            if (lowestUsageCpu <= lowestUsageMemory)
            {
                return lowestUsageCpuInstance;
            }
            else
            {
                return lowestUsageMemoryInstance;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }

        Random rd = new Random();
        var randomInstance = await db.Functions.Include(s => s.Instances).ToListAsync();
        await db.DisposeAsync();
        return randomInstance.First(s => s.Name.Equals(functionName)).Instances[rd.Next(0,randomInstance.Count)];
    }

    public async void Update()
    {
        var db = await _dbContextFactory.CreateDbContextAsync();
        foreach (var function in db.Functions.Include(s => s.Instances))
        {
            if (function.Instances.Count != 0)
            {
                foreach (var functionInstance in function.Instances)
                {
                    if (await IsOverloaded(functionInstance!.InstanceId))
                    {
                        await _functionManager.RunInstance(function.Name);
                    }
                    else if (!await IsOverloaded(functionInstance.InstanceId) && function.Instances.Count > 1)
                    {
                        _functionManager.DeleteInstance(functionInstance);
                    }
                }
            }
            else
            {
                await _functionManager.RunInstance(function.Name);
            }
            
        }

        await db.DisposeAsync();
    }

    public async void Start()
    {
        var db = await _dbContextFactory.CreateDbContextAsync();
        foreach (var function in db.Functions.Include(s => s.Instances))
        {
            await _functionManager.RunInstance(function.Name);
        }

        await db.DisposeAsync();
        
        _serviceProvider.GetRequiredService<TimerManager>().StartExecuting();
    }
}