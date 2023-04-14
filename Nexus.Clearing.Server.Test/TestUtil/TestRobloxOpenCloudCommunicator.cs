using System.Collections.Generic;
using System.Threading.Tasks;
using Nexus.Clearing.Server.Communicator.Roblox;

namespace Nexus.Clearing.Server.Test.TestUtil;

public class TestRobloxOpenCloudCommunicator : IRobloxOpenCloudCommunicator
{
    public enum OpenCloudCommunicatorCall
    {
        DeleteKeyAsync,
    }

    /// <summary>
    /// Previous calls made to the communicator.
    /// </summary>
    public List<(long, string, string, OpenCloudCommunicatorCall)> Calls = new List<(long, string, string, OpenCloudCommunicatorCall)>();
    
    /// <summary>
    /// Deletes a DataStore key.
    /// </summary>
    /// <param name="gameId">Id of the game to delete from.</param>
    /// <param name="apiKey">Roblox Open Cloud API key to use.</param>
    /// <param name="dataStoreName">Name of the DataStore to delete from.</param>
    /// <param name="dataStoreKey">Key of the DataStore to delete.</param>
    public Task<bool> DeleteKeyAsync(long gameId, string apiKey, string dataStoreName, string dataStoreKey)
    {
        this.Calls.Add((gameId, dataStoreName, dataStoreKey, OpenCloudCommunicatorCall.DeleteKeyAsync));
        return Task.FromResult(!dataStoreKey.Contains("NoData"));
    }
}