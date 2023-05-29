using System.Runtime.InteropServices;
using Docker.DotNet;
using Docker.DotNet.Models;
using Functions.Services.Interfaces;
using Environment = Functions.Data.Environment;

namespace Functions.Services;

public class DockerManager : IDockerManager
{
    private readonly ILogger<DockerManager> _logger;
    private readonly DockerClient _docker;
    public DockerManager(ILogger<DockerManager> logger)
    {
        _logger = logger;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _docker = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
        }
        else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            _docker = new DockerClientConfiguration(new Uri("unix:///var/run/dockerManager.sock")).CreateClient();
        }
        else
        {
            throw new Exception("This Platform is not Supported");
        }
    }

    public async Task<IList<ContainerListResponse>> GetContainers()
    {
        //TODO: Add Log Message
        IList<ContainerListResponse> containers = await _docker.Containers.ListContainersAsync(
            new ContainersListParameters(){
                Limit = 10,
            });
        return containers;
    }

    public async Task<ContainerResponse> CreateContainer(string image, List<Environment> envList)
    {
        var createContainerParameters = new CreateContainerParameters()
        {
            Image = image,
            Name = "FN-" + new Random().Next(10000, 99999),
            /*Env = envList,*/ //TODO: Parse the envs
            NetworkingConfig = new NetworkingConfig()
            {
                
            }
        };
        var container = await _docker.Containers.CreateContainerAsync(createContainerParameters);
        
        return new ContainerResponse(createContainerParameters.Name, container.ID);
    }

    public async void ConnectNetwork(string name, string containerId)
    {
        var networkConnectParameters = new NetworkConnectParameters()
        {
            Container = containerId
        };
        await _docker.Networks.ConnectNetworkAsync(await GetNetworkId(name), networkConnectParameters);
    }

    public async void StartContainer(string containerId)
    {
                
        var containerStartParameters = new ContainerStartParameters();
        await _docker.Containers.StartContainerAsync(containerId, containerStartParameters);
    }

    private async Task<string?> GetNetworkId(string name)
    {
        var response = await _docker.Networks.ListNetworksAsync();

        foreach (var networkResponse in response)
        {
            if (networkResponse.Name.Equals(name))
            {
                return networkResponse.ID;
            }
        }

        return null;
    }

    public async void DeleteContainer(string containerId)
    {
        var parameters = new ContainerRemoveParameters()
        {
            Force = true
        };
        await _docker.Containers.RemoveContainerAsync(containerId, parameters);
    }

    public async Task<bool> IsRunning(string containerId)
    {
        var containers = await GetContainers();
        foreach (var containerListResponse in containers)
        {
            if (containerListResponse.ID.Equals(containerId))
            {
                return containerListResponse.State.Equals("Running");
            }
        }

        return false;
    }
}

public class ContainerResponse
{
    public ContainerResponse(string name, string id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Id = id ?? throw new ArgumentNullException(nameof(id));
    }

    public string Name { get; set; }
    public string Id { get; set; }
}