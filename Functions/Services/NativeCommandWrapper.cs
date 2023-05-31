using System.Diagnostics;
using Functions.Services.Interfaces;

namespace Functions.Services;

public class NativeCommandWrapper : INativeCommandWrapper
{
    private readonly ILogger<NativeCommandWrapper> _logger;
    public NativeCommandWrapper(ILogger<NativeCommandWrapper> logger)
    {
        _logger = logger;
    }
    
    public async Task<string> GetContainerIdSelf()
    {
        Process proc = new();
        proc.StartInfo.FileName = "head";
        proc.StartInfo.Arguments = "-1 /proc/self/cgroup";
        proc.StartInfo.RedirectStandardOutput = true;
        proc.Start();

        ArgumentNullException.ThrowIfNull(proc);
        string output = await proc.StandardOutput.ReadToEndAsync();
        _logger.LogDebug(output.Substring(24));
        await proc.WaitForExitAsync();
        return (output.Substring(24));
    }
}