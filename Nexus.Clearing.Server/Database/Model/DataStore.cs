using System.ComponentModel.DataAnnotations;

namespace Nexus.Clearing.Server.Database.Model;

public class DataStore
{
    /// <summary>
    /// Id of the entry.
    /// </summary>
    [Key]
    public long Id { get; set; }
    
    /// <summary>
    /// Id of the game to clear.
    /// </summary>
    [Required]
    public long GameId { get; set; }

    /// <summary>
    /// Display name of the game to show in the logs.
    /// </summary>
    [Required]
    public string DisplayName { get; set; } = null!;

    /// <summary>
    /// Name of the DataStore to clear.
    /// </summary>
    [Required]
    public string DataStoreName { get; set; } = null!;

    /// <summary>
    /// Key of the DataStore to clear.
    /// </summary>
    [Required]
    public string DataStoreKey { get; set; } = null!;
}