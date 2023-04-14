using System.ComponentModel.DataAnnotations;
using Nexus.Clearing.Server.Enum;

namespace Nexus.Clearing.Server.Database.Model;

public class RobloxUser
{
    /// <summary>
    /// Id of the entry.
    /// </summary>
    [Key]
    public long Id { get; set; }
    
    /// <summary>
    /// Id of the user to clear.
    /// </summary>
    [Required]
    public ulong UserId { get; set; }

    /// <summary>
    /// Game ids to clear as a comma separated list.
    /// </summary>
    [Required]
    public string GameIds { get; set; } = null!;

    /// <summary>
    /// Time the entry was added.
    /// </summary>
    [Required]
    public DateTime RequestedTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Last time the status updated.
    /// </summary>
    [Required]
    public DateTime LastUpdateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Status of the clearing request.
    /// </summary>
    [Required]
    public ClearingState Status { get; set; } = ClearingState.Pending;

    /// <summary>
    /// Returns the game ids for a user.
    /// </summary>
    /// <returns>List of game ids for the user.</returns>
    public List<long> GetGameIds()
    {
        var gameIds = new List<long>();
        foreach (var gameIdString in this.GameIds.Split(","))
        {
            if (!long.TryParse(gameIdString, out var gameId)) continue;
            gameIds.Add(gameId);
        }
        return gameIds;
    }

    /// <summary>
    /// Sets the stored game ids.
    /// </summary>
    /// <param name="gameIds">Game ids to store.</param>
    public void SetGameIds(IEnumerable<long> gameIds) => this.GameIds = string.Join(",", gameIds);
}