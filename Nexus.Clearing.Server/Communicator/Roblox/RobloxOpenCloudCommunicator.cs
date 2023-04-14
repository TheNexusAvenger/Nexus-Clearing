using System.Net;
using Newtonsoft.Json;
using Nexus.Clearing.Server.Model.Response;

namespace Nexus.Clearing.Server.Communicator.Roblox;

public class RobloxOpenCloudCommunicator : IRobloxOpenCloudCommunicator
{
    /// <summary>
    /// Base URL used for Open Cloud.
    /// </summary>
    public const string BaseUrl = "https://apis.roblox.com";
    
    /// <summary>
    /// Static HTTP client for making requests.
    /// </summary>
    private static readonly HttpClient HttpClient = new HttpClient();

    /// <summary>
    /// Fetches the game id for a place id, if it exists.
    /// </summary>
    /// <param name="placeId">Place id to search with.</param>
    /// <returns>Game id that contains the place id, if any.</returns>
    public async Task<long?> GetGameIdFromPlaceIdAsync(long placeId)
    {
        return JsonConvert.DeserializeObject<UniverseResponse>(await (await HttpClient.GetAsync($"https://apis.roblox.com/universes/v1/places/{placeId}/universe")).Content.ReadAsStringAsync())?.UniverseId;
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
        var url = $"{BaseUrl}/datastores/v1/universes/{gameId}/standard-datastores/datastore/entries/entry?datastoreName={dataStoreName}&entryKey={dataStoreKey}";
        var request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.Add("x-api-key", apiKey);
        var response = await HttpClient.SendAsync(request);
        if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.NotFound)
        {
            throw new InvalidDataException($"Unable to determine if game {gameId} deleted key {dataStoreKey} in {dataStoreName} (HTTP {response.StatusCode}) {await response.Content.ReadAsStringAsync()}");
        }
        return response.StatusCode == HttpStatusCode.NoContent;
    }
}