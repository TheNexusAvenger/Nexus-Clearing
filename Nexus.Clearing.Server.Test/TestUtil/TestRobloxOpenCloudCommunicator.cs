using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nexus.Clearing.Server.Communicator.Roblox;

namespace Nexus.Clearing.Server.Test.TestUtil;

public class TestRobloxOpenCloudCommunicator : IRobloxOpenCloudCommunicator
{
    public enum OpenCloudCommunicatorCall
    {
        GetGameIdFromPlaceIdAsync,
        DeleteKeyAsync,
    }

    /// <summary>
    /// Previous calls made to the communicator.
    /// </summary>
    public List<(long, string, string, OpenCloudCommunicatorCall)> Calls = new List<(long, string, string, OpenCloudCommunicatorCall)>();

    /// <summary>
    /// If true, an exception will be thrown on call.
    /// </summary>
    public bool ThrowExceptionOnCall { get; set; } = false;
    
    /// <summary>
    /// Fetches the game id for a place id, if it exists.
    /// </summary>
    /// <param name="placeId">Place id to search with.</param>
    /// <returns>Game id that contains the place id, if any.</returns>
    public Task<long?> GetGameIdFromPlaceIdAsync(long placeId)
    {
        this.Calls.Add((placeId, null, null, OpenCloudCommunicatorCall.GetGameIdFromPlaceIdAsync)!);
        return Task.FromResult(placeId == 10 ? (long?) 1L : null);
    }
    
    /// <summary>
    /// Deletes a DataStore key.
    /// </summary>
    /// <param name="gameId">Id of the game to delete from.</param>
    /// <param name="apiKey">Roblox Open Cloud API key to use.</param>
    /// <param name="dataStoreName">Name of the DataStore to delete from.</param>
    /// <param name="dataStoreKey">Key of the DataStore to delete.</param>
    public Task<bool> DeleteKeyAsync(long gameId, string apiKey, string dataStoreName, string dataStoreKey)
    {
        if (this.ThrowExceptionOnCall)
        {
            throw new Exception("Test exception");
        }
        this.Calls.Add((gameId, dataStoreName, dataStoreKey, OpenCloudCommunicatorCall.DeleteKeyAsync));
        return Task.FromResult(!dataStoreKey.Contains("NoData"));
    }
}