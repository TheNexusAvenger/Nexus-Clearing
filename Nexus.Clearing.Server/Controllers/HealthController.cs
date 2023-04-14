using Microsoft.AspNetCore.Mvc;

namespace Nexus.Clearing.Server.Controllers;

[ApiController]
public class HealthController
{
    [HttpGet]
    [Route("health")]
    public string Get()
    {
        return "Up";
    }
}