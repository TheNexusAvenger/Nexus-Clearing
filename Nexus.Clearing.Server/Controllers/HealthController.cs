using Microsoft.EntityFrameworkCore;
using Nexus.Clearing.Server.Communicator.Roblox;
using Nexus.Clearing.Server.Database;
using Nexus.Clearing.Server.Model.Response;

namespace Nexus.Clearing.Server.Controllers;

public class HealthController
{
    /// <summary>
    /// Communicator used for Roblox Open Cloud.
    /// </summary>
    private readonly IRobloxOpenCloudCommunicator _robloxOpenCloudCommunicator;
    
    /// <summary>
    /// Creates a health controller.
    /// </summary>
    /// <param name="robloxOpenCloudCommunicator">Communicator used for Roblox Open Cloud.</param>
    public HealthController(IRobloxOpenCloudCommunicator robloxOpenCloudCommunicator)
    {
        this._robloxOpenCloudCommunicator = robloxOpenCloudCommunicator;
    }
    
    /// <summary>
    /// Creates a health controller.
    /// </summary>
    public HealthController() : this(new RobloxOpenCloudCommunicator())
    {
        
    }
    
    /// <summary>
    /// Handles a health check request.
    /// </summary>
    public async Task<HealthCheckResponse> PerformHealthCheckAsync()
    {
        // Perform a GetAsync request on all the DataStores to test connectivity.
        var openCloudApiKeyIssues = 0;
        await using var context = new SqliteContext();
        foreach (var datastore in context.DataStores)
        {
            // Get the Open Cloud API key.
            var openCloudKey = await context.RobloxGameKeys.FirstOrDefaultAsync(gameKey => gameKey.GameId == datastore.GameId);
            if (openCloudKey == null || openCloudKey.OpenCloudApiKey == null)
            {
                Logger.Warn($"Game {datastore.GameId} does not have an Open Cloud API key configured.");
                openCloudApiKeyIssues += 1;
                continue;
            }
            
            // Try deleting a key.
            var dataStoreName = datastore.DataStoreName.Replace("{UserId}", "1");
            var key = "NEXUS_CLEARING_TEST_" + new Guid();
            try
            {
                await this._robloxOpenCloudCommunicator.DeleteKeyAsync(datastore.GameId, openCloudKey.OpenCloudApiKey!, dataStoreName, key);
            }
            catch (Exception e)
            {
                openCloudApiKeyIssues += 1;
                Logger.Error($"An error occured getting {key} from datastore {dataStoreName} from game {datastore.GameId}.\n{e}");
            }
        }
        
        // Return the response.
        var response = new HealthCheckResponse()
        {
            Status = (openCloudApiKeyIssues > 0 ? HealthCheckResultStatus.Down : HealthCheckResultStatus.Up),
            OpenCloudIssues = openCloudApiKeyIssues,
        };
        return response;
    }
}