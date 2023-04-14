namespace Nexus.Clearing.Server.Model.Request;

public class RightToErasureRequestEventPayload
{
    /// <summary>
    /// User id to clear.
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Game ids the player needs cleared.
    /// </summary>
    public List<long> GameIds { get; set; } = null!;
}