namespace Nexus.Clearing.Server.Communicator.Roblox;

public interface IRobloxOpenCloudCommunicator
{
    /// <summary>
    /// Deletes a DataStore key.
    /// </summary>
    /// <param name="gameId">Id of the game to delete from.</param>
    /// <param name="apiKey">Roblox Open Cloud API key to use.</param>
    /// <param name="dataStoreName">Name of the DataStore to delete from.</param>
    /// <param name="dataStoreKey">Key of the DataStore to delete.</param>
    /// <returns>Whether the key had any data.</returns>
    public Task<bool> DeleteKeyAsync(long gameId, string apiKey, string dataStoreName, string dataStoreKey);
}