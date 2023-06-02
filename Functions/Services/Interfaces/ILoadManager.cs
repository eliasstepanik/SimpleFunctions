namespace Functions.Services.Interfaces;

public interface ILoadManager
{
    public Task<HttpResponseMessage> HandleRequest(string functionName, HttpMethod method, string body = "");
    public void Update();
    public void Start();
}