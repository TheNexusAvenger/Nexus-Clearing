using Microsoft.EntityFrameworkCore;
using Nexus.Clearing.Server.Communicator.Roblox;
using Nexus.Clearing.Server.Database;
using Nexus.Clearing.Server.Enum;

namespace Nexus.Clearing.Server.Util;

public static class ClearDataStores
{
    /// <summary>
    /// Communicator used for making requests to Roblox Open cloud.
    /// </summary>
    public static IRobloxOpenCloudCommunicator RobloxOpenCloudCommunicator { get; set; } = new CachingRobloxOpenCloudCommunicator();
    
    /// <summary>
    /// Clears the data of a Roblox user.
    /// </summary>
    /// <param name="userId">Id of the user to clear.</param>
    public static async Task ClearPendingUserAsync(long userId)
    {
        // Get the user.
        await using var context = new SqliteContext();
        var user = context.RobloxUsers.First(user => user.UserId == userId && user.Status != ClearingState.Complete);
        
        try
        {
            // Delete the DataStores.
            Logger.Info($"Clearing data for {userId}.");
            foreach (var initialGameId in user.GetGameIds())
            {
                // Ignore the game if there is no DataStore keys.
                var gameId = initialGameId;
                var dataStoreKeys = await context.DataStores.Where(dataStore => dataStore.GameId == gameId).ToListAsync();
                if (dataStoreKeys.Count == 0)
                {
                    var gameIdFromPlaceId = await RobloxOpenCloudCommunicator.GetGameIdFromPlaceIdAsync(gameId);
                    if (gameIdFromPlaceId != null)
                    {
                        dataStoreKeys = await context.DataStores.Where(dataStore => dataStore.GameId == gameIdFromPlaceId.Value).ToListAsync();
                        if (dataStoreKeys.Count != 0)
                        {
                            Logger.Debug($"Place id {gameId} was converted to game id {gameIdFromPlaceId.Value}.");
                            gameId = gameIdFromPlaceId.Value;
                        }
                    }
                    if (dataStoreKeys.Count == 0)
                    {
                        Logger.Debug($"User {userId} was meant to be cleared for {gameId} but has no DataStore keys.");
                        continue;
                    }
                }
                
                // Throw an exception if there is no Open Cloud key.
                var openCloudKey = await context.RobloxGameKeys.FirstOrDefaultAsync(gameKey => gameKey.GameId == gameId);
                if (openCloudKey?.OpenCloudApiKey == null)
                {
                    throw new MissingFieldException($"{gameId} has no configured Open Cloud API key.");
                }
                
                // Clear the DataStore keys.
                foreach (var dataStoreKey in dataStoreKeys)
                {
                    var dataStoreName = dataStoreKey.DataStoreName.Replace("{UserId}", userId.ToString());
                    var key = dataStoreKey.DataStoreKey.Replace("{UserId}", userId.ToString());
                    Logger.Debug($"Deleting key {key} in DataStore {dataStoreName} for user {userId}.");
                    if (await RobloxOpenCloudCommunicator.DeleteKeyAsync(dataStoreKey.GameId, openCloudKey.OpenCloudApiKey, dataStoreName, key))
                    {
                        Logger.Debug($"Deleted key {key} in DataStore {dataStoreName} for user {userId}.");
                    }
                    else
                    {
                        Logger.Debug($"User {userId} has no DataStore key {key} in DataStore {dataStoreName}.");
                    }
                }
            }
            
            // Mark the user as cleared.
            Logger.Info($"User {userId} was cleared.");
            user.Status = ClearingState.Complete;
        }
        catch (Exception e)
        {
            Logger.Error($"An exception occurred trying to clear user {userId}:\n{e}");
            user.Status = ClearingState.AwaitingRetry;
        }
        user.LastUpdateTime = DateTime.Now;
        await context.SaveChangesAsync();
    }
    
    /// <summary>
    /// Clears the data for the pending users in the database.
    /// </summary>
    public static async Task ClearPendingUsersAsync()
    {
        await using var context = new SqliteContext();
        var usersToClear = await context.RobloxUsers.Where(user => user.Status != ClearingState.Complete).ToListAsync();
        if (usersToClear.Count == 0)
        {
            Logger.Debug("There are currently no users to clear.");
        }
        else
        {
            Logger.Info($"Attempting to clear {usersToClear.Count} users.");
        }
        foreach (var user in usersToClear)
        {
            await ClearPendingUserAsync(user.UserId);
        }
    }
}