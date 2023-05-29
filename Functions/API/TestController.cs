using System.Collections;
using Docker.DotNet.Models;
using Functions.Services;
using Functions.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Functions.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly IDockerManager _dockerManager;

    public TestController(ILogger<TestController> logger, IDockerManager dockerManager)
    {
        _logger = logger;
        _dockerManager = dockerManager;
    }

    [HttpGet(Name = "Containers")]
    public async Task<IEnumerable<ContainerListResponse>> Get()
    {
        return await _dockerManager.GetContainers();
    }
}