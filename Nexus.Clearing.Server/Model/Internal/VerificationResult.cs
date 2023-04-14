namespace Nexus.Clearing.Server.Model.Internal;

public class VerificationResult
{
    /// <summary>
    /// Timestamp that was attempted to be verified.
    /// </summary>
    public string? Timestamp { get; set; }
    
    /// <summary>
    /// Signature that was attempted to be verified.
    /// </summary>
    public string? Signature { get; set; }

    /// <summary>
    /// Body of the request that was attempted to be verified.
    /// </summary>
    public string Body { get; set; } = null!;
    
    /// <summary>
    /// Whether the signature if valid.
    /// </summary>
    public bool Valid { get; set; }
}