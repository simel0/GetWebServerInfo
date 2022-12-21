using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;

namespace GetWebServerInfo.Controllers;

[ApiController]
[Route("[controller]")]
public class WebServerController : ControllerBase
{
    private readonly ILogger<WebServerController> _logger;
    private readonly IServiceProvider? _service;
    public WebServerController(ILogger<WebServerController> logger, IServiceProvider provider)
    {
        _logger = logger;
        _service = provider;
    }

    [HttpGet(Name = "GetInfo")]
    public IActionResult Get()
    {
        var model = new WebServerInfo(_service);
        //var model1 = new WebServerInfo(); => miss some information

        return new ObjectResult(model);
    }
}