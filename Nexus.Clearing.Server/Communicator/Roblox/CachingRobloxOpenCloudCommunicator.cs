namespace Nexus.Clearing.Server.Communicator.Roblox;

public class CachingRobloxOpenCloudCommunicator : IRobloxOpenCloudCommunicator
{
    /// <summary>
    /// Base communicator used for Roblox.
    /// </summary>
    public IRobloxOpenCloudCommunicator Communicator { get; set; } = new RobloxOpenCloudCommunicator();

    /// <summary>
    /// Cache of the game ids for the place ids.
    /// </summary>
    private readonly Dictionary<long, long?> _gameIdCache = new Dictionary<long, long?>();

    /// <summary>
    /// Fetches the game id for a place id, if it exists.
    /// </summary>
    /// <param name="placeId">Place id to search with.</param>
    /// <returns>Game id that contains the place id, if any.</returns>
    public async Task<long?> GetGameIdFromPlaceIdAsync(long placeId)
    {
        if (!this._gameIdCache.ContainsKey(placeId))
        {
            this._gameIdCache[placeId] = await this.Communicator.GetGameIdFromPlaceIdAsync(placeId);
        }
        return this._gameIdCache[placeId];
    }
    
    /// <summary>
    /// Deletes a DataStore key.
    /// </summary>
    /// <param name="gameId">Id of the game to delete from.</param>
    /// <param name="apiKey">Roblox Open Cloud API key to use.</param>
    /// <param name="dataStoreName">Name of the DataStore to delete from.</param>
    /// <param name="dataStoreKey">Key of the DataStore to delete.</param>
    /// <returns>Whether any data was deleted.</returns>
    public async Task<bool> DeleteKeyAsync(long gameId, string apiKey, string dataStoreName, string dataStoreKey)
    {
        return await this.Communicator.DeleteKeyAsync(gameId, apiKey, dataStoreName, dataStoreKey);
    }
}