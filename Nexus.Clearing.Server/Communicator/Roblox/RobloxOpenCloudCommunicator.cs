namespace Nexus.Clearing.Server.Communicator.Roblox;

public class RobloxOpenCloudCommunicator : IRobloxOpenCloudCommunicator
{
    /// <summary>
    /// Checks if a DataStore currently has data.
    /// </summary>
    /// <param name="gameId">Id of the game to check.</param>
    /// <param name="dataStoreName">Name of the DataStore to check.</param>
    /// <param name="dataStoreKey">Key of the DataStore to check.</param>
    /// <returns>Whether the DataStore key has any data.</returns>
    public Task<bool> HasDataAsync(long gameId, string dataStoreName, string dataStoreKey)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Deletes a DataStore key.
    /// </summary>
    /// <param name="gameId">Id of the game to delete from.</param>
    /// <param name="dataStoreName">Name of the DataStore to delete from.</param>
    /// <param name="dataStoreKey">Key of the DataStore to delete.</param>
    public Task DeleteKeyAsync(long gameId, string dataStoreName, string dataStoreKey)
    {
        throw new NotImplementedException();
    }
}