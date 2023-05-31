using Microsoft.AspNetCore.Mvc;

namespace Functions.Services.Interfaces;

public interface IExternalEndpointManager
{
    public Task<HttpResponseMessage> Get(string hostname);
    public Task<HttpResponseMessage> Post(string hostname, string body);
    public Task<HttpResponseMessage> Delete(string hostname);
    public Task<HttpResponseMessage> Patch(string hostname, string body);
    public Task<HttpResponseMessage> Put(string hostname, string body);
}