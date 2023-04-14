using System.Collections.Generic;
using System.Threading.Tasks;
using Nexus.Clearing.Server.Communicator.Roblox;

namespace Nexus.Clearing.Server.Test.TestUtil;

public class TestRobloxOpenCloudCommunicator : IRobloxOpenCloudCommunicator
{
    public enum OpenCloudCommunicatorCall
    {
        HasDataAsync,
        DeleteKeyAsync,
    }

    /// <summary>
    /// Previous calls made to the communicator.
    /// </summary>
    public List<(long, string, string, OpenCloudCommunicatorCall)> Calls = new List<(long, string, string, OpenCloudCommunicatorCall)>();
    
    /// <summary>
    /// Checks if a DataStore currently has data.
    /// </summary>
    /// <param name="gameId">Id of the game to check.</param>
    /// <param name="dataStoreName">Name of the DataStore to check.</param>
    /// <param name="dataStoreKey">Key of the DataStore to check.</param>
    /// <returns>Whether the DataStore key has any data.</returns>
    public Task<bool> HasDataAsync(long gameId, string dataStoreName, string dataStoreKey)
    {
        this.Calls.Add((gameId, dataStoreName, dataStoreKey, OpenCloudCommunicatorCall.HasDataAsync));
        return Task.FromResult(!dataStoreKey.Contains("NoData"));
    }

    /// <summary>
    /// Deletes a DataStore key.
    /// </summary>
    /// <param name="gameId">Id of the game to delete from.</param>
    /// <param name="dataStoreName">Name of the DataStore to delete from.</param>
    /// <param name="dataStoreKey">Key of the DataStore to delete.</param>
    public Task DeleteKeyAsync(long gameId, string dataStoreName, string dataStoreKey)
    {
        this.Calls.Add((gameId, dataStoreName, dataStoreKey, OpenCloudCommunicatorCall.DeleteKeyAsync));
        return Task.CompletedTask;
    }
}