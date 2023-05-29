using Microsoft.AspNetCore.Mvc;

namespace Functions.Services.Interfaces;

public interface IExternalEndpointManager
{
    public Task<string> Get(string hostname);
    public Task<string> Post(string hostname, string body);
    public Task<string> Delete(string hostname);
    public Task<string> Patch(string hostname, string body);
}