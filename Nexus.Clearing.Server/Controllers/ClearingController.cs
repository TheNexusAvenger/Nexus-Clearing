using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexus.Clearing.Server.Database;
using Nexus.Clearing.Server.Database.Model;
using Nexus.Clearing.Server.Enum;
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
            return new ObjectResult("InvalidSignature")
            {
                StatusCode = 401,
            };
        }
        
        // Return if the user is already stored and pending clearing.
        await using var context = new SqliteContext();
        var existingUser = await context.RobloxUsers.FirstOrDefaultAsync(user => user.UserId == request.EventPayload.UserId);
        if (existingUser != null && existingUser.Status != ClearingState.Complete)
        {
            Logger.Info($"Request to delete user {request.EventPayload.UserId} was already queued.");
            return new ObjectResult("AlreadyQueued")
            {
                StatusCode = 200,
            };
        }
        
        // Store the player.
        if (existingUser == null)
        {
            var user = new RobloxUser()
            {
                UserId = request.EventPayload.UserId,
            };
            user.SetGameIds(request.EventPayload.GameIds);
            await context.RobloxUsers.AddAsync(user);
            existingUser = user;
        }
        else
        {
            var gameIds = existingUser.GetGameIds();
            foreach (var gameId in request.EventPayload.GameIds ?? new List<long>())
            {
                gameIds.Add(gameId);
            }
            existingUser.SetGameIds(gameIds);
            existingUser.LastUpdateTime = DateTime.Now;
            existingUser.Status = ClearingState.Pending;
        }
        Logger.Info($"Adding deletion request for user {request.EventPayload.UserId} for game ids {existingUser.GameIds}");
        await context.SaveChangesAsync();
        
        // Return success.
        return new ObjectResult("Success")
        {
            StatusCode = 200,
        };
    }
}