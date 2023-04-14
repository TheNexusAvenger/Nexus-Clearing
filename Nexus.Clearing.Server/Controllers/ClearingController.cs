using Microsoft.AspNetCore.Mvc;

namespace Nexus.Clearing.Server.Controllers;

[ApiController]
[Route("/clearing")]
public class ClearingController : ControllerBase
{
    /// <summary>
    /// Route for Roblox's webhook.
    /// </summary>
    [HttpPost]
    [Route("roblox")]
    public async Task<string> HandleRobloxWebhook()
    {
        // TODO: Actually handle webhook.
        var content = await new StreamReader(this.Request.Body).ReadToEndAsync();
        Logger.Debug("Starting request handling.");
        foreach (var header in this.Request.Headers)
        {
            Logger.Debug($"Header {header.Key} = {header.Value}");
        }
        Logger.Debug($"Body: {content}");
        return "Success";
    }
}