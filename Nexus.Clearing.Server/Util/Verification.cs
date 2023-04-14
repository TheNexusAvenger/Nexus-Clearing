using System.Security.Cryptography;
using System.Text;
using Nexus.Clearing.Server.Database;
using Nexus.Clearing.Server.Model.Internal;

namespace Nexus.Clearing.Server.Util;

public static class Verification
{
    /// <summary>
    /// Builds a verification result.
    /// </summary>
    /// <param name="signatureHeader">Signature header of the request.</param>
    /// <param name="body">Body of the request.</param>
    /// <returns>Initial verification result for a request.</returns>
    private static VerificationResult BuildResult(string signatureHeader, string body)
    {
        var verificationResult = new VerificationResult()
        {
            Body = body,
        };
        foreach (var headerPart in signatureHeader.Split(","))
        {
            if (!headerPart.Contains("=")) return verificationResult;
            var splitHeaderParts = headerPart.Split("=", 2);
            if (splitHeaderParts[0] == "t")
            {
                verificationResult.Timestamp = splitHeaderParts[1];
            } else if (splitHeaderParts[0] == "v1")
            {
                verificationResult.Signature = splitHeaderParts[1];
            }
        }
        return verificationResult;
    }
    
    /// <summary>
    /// Determines if a signature matches a request.
    /// </summary>
    /// <param name="signatureHeader">Header containing the signature.</param>
    /// <param name="secret">Secret used for the HMAC SHA256 hash.</param>
    /// <param name="body">Body of the request part of the signature.</param>
    /// <returns>Result for the verification.</returns>
    public static VerificationResult VerifyRequest(string signatureHeader, string secret, string body)
    {
        // Get the timestamp and signature.
        var verificationResult = BuildResult(signatureHeader, body);
        if (verificationResult.Timestamp == null || verificationResult.Signature == null)
        {
            return verificationResult;
        }
        
        // Compute the hash for the body.
        using var sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var inputText = $"{verificationResult.Timestamp}.{body}";
        var newSignature = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(inputText)));
        
        // Return if the signatures match.
        verificationResult.Valid = verificationResult.Signature == newSignature;
        return verificationResult;
    }
    
    /// <summary>
    /// Determines if a signature matches a request with the secret for any experience.
    /// </summary>
    /// <param name="request">Request to verify against.</param>
    /// <param name="body">Body of the request part of the signature.</param>
    /// <returns>Result for the verification.</returns>
    public static async Task<VerificationResult> VerifyRequestAsync(HttpRequest request, string body)
    {
        if (!request.Headers.TryGetValue("roblox-signature", out var headerValues)) return new VerificationResult() {Body = body};
        if (headerValues.Count == 0) return new VerificationResult() {Body = body};
        await using var context = new SqliteContext();
        foreach (var secret in context.RobloxGameKeys.Select(keyEntry => keyEntry.WebHookSecret).ToHashSet())
        {
            var result = VerifyRequest(headerValues[0]!, secret, body);
            if (!result.Valid) continue;
            return result;
        }
        return BuildResult(headerValues[0]!, body);;
    }
}