namespace Functions.Services.Interfaces;

public interface INativeCommandWrapper
{
    public Task<string> GetContainerIdSelf();
}