using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;

namespace GetWebServerInfo.Controllers;

[ApiController]
[Route("[controller]")]
public class WebServerController : ControllerBase
{
    private readonly ILogger<WebServerController> _logger;

    public WebServerController(ILogger<WebServerController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetInfo")]
    public IActionResult Get()
    {
        var model = new WebServerInfo
        {

        };
        return new ObjectResult(model);
    }
}