using System.Text.Json.Serialization;

namespace Nexus.Clearing.Server.Model.Response;

public class UniverseResponse
{
    /// <summary>
    /// Id of the game the place is part of.
    /// </summary>
    public long? UniverseId { get; set; }
}

[JsonSerializable(typeof(UniverseResponse))]
[JsonSourceGenerationOptions(WriteIndented=true, IncludeFields = true)]
internal partial class UniverseResponseJsonContext : JsonSerializerContext
{
}
