using Docker.DotNet.Models;
using Functions.Services;
using Functions.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Functions.Controllers;

[ApiController]
[Route("[controller]")]
public class FunctionController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly FunctionManager _functionManager;

    public FunctionController(ILogger<TestController> logger, FunctionManager functionManager)
    {
        _logger = logger;
        _functionManager = functionManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateFunction(string functionName, string imageTag)
    {
        await _functionManager.CreateFunction(functionName, imageTag);
        return Ok();
    }


    [HttpPost("{functionName}")]
    public async Task<string> RunFunctionPost(string functionName,[FromBody] string text)
    {
        var response = await _functionManager.RunInstance(functionName,HttpMethod.Post, text);
        _logger.LogInformation(functionName);
        return response;
    }
    [HttpGet("{functionName}")]
    public async Task<string> RunFunctionGet(string functionName)
    {
        var response = await _functionManager.RunInstance(functionName,HttpMethod.Get);
        _logger.LogInformation(functionName);
        return response;
    }
    
    [HttpPatch("{functionName}")]
    public async Task<string> RunFunctionPatch(string functionName,[FromBody] string text)
    {
        var response = await _functionManager.RunInstance(functionName,HttpMethod.Patch, text);
        _logger.LogInformation(functionName);
        return response;
    }

    
    [HttpDelete("{functionName}/delete")]
    public async Task<IActionResult> DeleteFunction(string functionName)
    {
        await _functionManager.DeleteFunction(functionName);
        _logger.LogInformation(functionName);
        return Ok();
    }
}