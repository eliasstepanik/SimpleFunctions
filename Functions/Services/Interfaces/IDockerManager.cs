using Docker.DotNet.Models;
using Functions.Data;
using Environment = Functions.Data.Environment;

namespace Functions.Services.Interfaces;

public interface IDockerManager
{
    public Task<IList<ContainerListResponse>> GetContainers();
    public Task<ContainerResponse> CreateContainer(string image, List<Environment> envList);
    public void ConnectNetwork(string name, string containerId);
    public void StartContainer(string containerId);
    public void DeleteContainer(string containerId);
    public void CreateNetwork(string name);
    public Task<bool> IsRunning(string containerId);
}