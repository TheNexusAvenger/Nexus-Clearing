using System.ComponentModel.DataAnnotations;

namespace Nexus.Clearing.Server.Database.Model;

public class GameKey
{
    /// <summary>
    /// Id of the game the keys are for.
    /// </summary>
    [Key]
    public long GameId { get; set; }

    /// <summary>
    /// Secret used from the web hook to verify the identity from Roblox.
    /// </summary>
    [Required]
    public string WebHookSecret { get; set; } = null!;
    
    /// <summary>
    /// Open Cloud API key for clearing DataStores.
    /// </summary>
    public string? OpenCloudApiKey { get; set; }
}