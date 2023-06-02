using Docker.DotNet;
using Docker.DotNet.Models;

namespace TestFunction;

public class Test
{
    public static async Task<ContainerStatsResponse> GetLoad(string containerId)
    {
        ContainerStatsParameters parameters = new ContainerStatsParameters()
        {
            Stream = false,
        };
        var response = new ContainerStatsResponse();

        IProgress<ContainerStatsResponse> progress = new Progress<ContainerStatsResponse>(stats => { response = stats; });

        var _docker = new DockerClientConfiguration().CreateClient();
        await _docker.Containers.GetContainerStatsAsync(containerId,parameters, progress);
        return response;
    }
}