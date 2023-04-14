using System.Security.Cryptography;
using System.Text;

namespace Nexus.Clearing.Server.Util;

public class Verification
{
    /// <summary>
    /// Determines if a signature matches a request.
    /// </summary>
    /// <param name="signatureHeader">Header containing the signature.</param>
    /// <param name="secret">Secret used for the HMAC SHA256 hash.</param>
    /// <param name="body">Body of the request part of the signature.</param>
    /// <returns>Whether the signature is valid or not.</returns>
    public static bool VerifyRequest(string signatureHeader, string secret, string body)
    {
        // Get the timestamp and signature.
        var headerParameters = new Dictionary<string, string>();
        foreach (var headerPart in signatureHeader.Split(","))
        {
            if (!headerPart.Contains("=")) return false;
            var splitHeaderParts = headerPart.Split("=", 2);
            headerParameters[splitHeaderParts[0]] = splitHeaderParts[1];
        }
        if (!headerParameters.ContainsKey("t") || !headerParameters.ContainsKey("v1"))
        {
            return false;
        }
        var timestamp = headerParameters["t"];
        var signature = headerParameters["v1"];
        
        // Compute the hash for the body.
        using var sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var inputText = $"{timestamp}.{body}";
        var newSignature = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(inputText)));
        
        // Return if the signatures match.
        return signature == newSignature;
    }
    
    /// <summary>
    /// Determines if a signature matches a request.
    /// </summary>
    /// <param name="request">Request to verify against.</param>
    /// <param name="secret">Secret used for the HMAC SHA256 hash.</param>
    /// <param name="body">Body of the request part of the signature.</param>
    /// <returns>Whether the signature is valid or not.</returns>
    public static bool VerifyRequest(HttpRequest request, string secret, string body)
    {
        if (!request.Headers.TryGetValue("roblox-signature", out var headerValues)) return false;
        if (headerValues.Count == 0) return false;
        return VerifyRequest(headerValues[0]!, secret, body);
    }
}