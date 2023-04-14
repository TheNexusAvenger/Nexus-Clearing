using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Nexus.Clearing.Server.Database;
using Nexus.Clearing.Server.Database.Model;
using Nexus.Clearing.Server.Model.Request;
using Nexus.Clearing.Server.Util;

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
    public async Task<ObjectResult> HandleRobloxWebhook()
    {
        // Read the body of the request.
        var content = await new StreamReader(this.Request.Body).ReadToEndAsync();
        var request = JsonSerializer.Deserialize<RobloxNotification<RightToErasureRequestEventPayload>>(content)!;
        if (request.EventType != "RightToErasureRequest")
        {
            Logger.Warn($"Got request for {request.EventType} instead of RightToErasureRequest.");
            return new ObjectResult("InvalidEventType")
            {
                StatusCode = 400,
            };
        }
        
        // Verify the signature of the request.
        var verificationResult = await Verification.VerifyRequestAsync(this.Request, content);
        if (!verificationResult.Valid)
        {
            Logger.Warn($"Request had invalid signature.\nTimestamp: {verificationResult.Timestamp ?? "(None)"}\nSignature: {verificationResult.Signature ?? "(None)"}\nBody: {content}");
            return new ObjectResult("InvalidEventType")
            {
                StatusCode = 401,
            };
        }
        
        // Store the player.
        await using var context = new SqliteContext();
        var user = new RobloxUser()
        {
            UserId = request.EventPayload.UserId,
        };
        user.SetGameIds(request.EventPayload.GameIds);
        Logger.Info($"Adding deletion request for user {request.EventPayload.UserId} for game ids {user.GameIds}");
        await context.RobloxUsers.AddAsync(user);
        await context.SaveChangesAsync();

        // Start processing the deletion request in the background.
        // TODO
        
        // Return success.
        return new ObjectResult("Success");
    }
}