using System.Net;
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
    private readonly ILoadManager _loadManager;


    public FunctionController(ILogger<TestController> logger, FunctionManager functionManager, ILoadManager loadManager)
    {
        _logger = logger;
        _functionManager = functionManager;
        _loadManager = loadManager;
    }

    [HttpPost("{functionName}/edit")]
    public async Task<IActionResult> CreateFunction(string functionName, string imageTag)
    {
        await _functionManager.CreateFunction(functionName, imageTag);
        return Ok();
    }


    [HttpPost("{functionName}")]
    public async Task<IActionResult> RunFunctionPost(string functionName,[FromBody] string text)
    {
        var responseContext = await _loadManager.HandleRequest(functionName,HttpMethod.Post, text);
        
        if (responseContext.IsSuccessStatusCode)
        {
            //_logger.LogInformation(""); TODO: Write Log Message
            return Ok(await responseContext.Content.ReadAsStringAsync());
        }
        if(responseContext.StatusCode == HttpStatusCode.BadRequest)
        {
            return BadRequest(responseContext.ReasonPhrase);
        }

        return NoContent();
    }
    [HttpGet("{functionName}")]
    public async Task<IActionResult> RunFunctionGet(string functionName)
    {
        var responseContext = await _loadManager.HandleRequest(functionName,HttpMethod.Get);
        
        if (responseContext.IsSuccessStatusCode)
        {
            //_logger.LogInformation(""); TODO: Write Log Message
            return Ok(await responseContext.Content.ReadAsStringAsync());
        }
        if(responseContext.StatusCode == HttpStatusCode.BadRequest)
        {
            return BadRequest(responseContext.ReasonPhrase);
        }

        return NoContent();
    }
    
    [HttpPatch("{functionName}")]
    public async Task<IActionResult> RunFunctionPatch(string functionName,[FromBody] string text)
    {
        var responseContext = await _loadManager.HandleRequest(functionName,HttpMethod.Patch, text);
        
        if (responseContext.IsSuccessStatusCode)
        {
            //_logger.LogInformation(""); TODO: Write Log Message
            return Ok(await responseContext.Content.ReadAsStringAsync());
        }
        if(responseContext.StatusCode == HttpStatusCode.BadRequest)
        {
            return BadRequest(responseContext.ReasonPhrase);
        }

        return NoContent();
    }
    
    [HttpPut("{functionName}")]
    public async Task<IActionResult> RunFunctionPut(string functionName,[FromBody] string text)
    {
        var responseContext = await _loadManager.HandleRequest(functionName,HttpMethod.Put, text);

        if (responseContext.IsSuccessStatusCode)
        {
            //_logger.LogInformation(""); TODO: Write Log Message
            return Ok(await responseContext.Content.ReadAsStringAsync());
        }
        if(responseContext.StatusCode == HttpStatusCode.BadRequest)
        {
            return BadRequest(responseContext.ReasonPhrase);
        }

        return NoContent();
    }
    
    [HttpDelete("{functionName}")]
    public async Task<IActionResult> RunFunctionDelete(string functionName,[FromBody] string text)
    {
        var responseContext = await _loadManager.HandleRequest(functionName,HttpMethod.Delete, text);

        if (responseContext.IsSuccessStatusCode)
        {
            //_logger.LogInformation(""); TODO: Write Log Message
            return Ok(await responseContext.Content.ReadAsStringAsync());
        }
        if(responseContext.StatusCode == HttpStatusCode.BadRequest)
        {
            return BadRequest(responseContext.ReasonPhrase);
        }

        return NoContent();
    }

    
    [HttpDelete("{functionName}/edit")]
    public async Task<IActionResult> DeleteFunction(string functionName)
    {
        await _functionManager.DeleteFunction(functionName);
        _logger.LogInformation(functionName);
        return Ok();
    }
}