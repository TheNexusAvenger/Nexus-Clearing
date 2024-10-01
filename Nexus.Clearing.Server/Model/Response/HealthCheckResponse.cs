using System.Text.Json.Serialization;

namespace Nexus.Clearing.Server.Model.Response;

public enum HealthCheckResultStatus
{
    Up,
    Down,
}

public class HealthCheckResponse
{
    /// <summary>
    /// Status of the health check for the application.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<HealthCheckResultStatus>))]
    [JsonPropertyName("status")]
    public HealthCheckResultStatus Status { get; set; } = HealthCheckResultStatus.Up;

    /// <summary>
    /// Total number of Open Cloud API key issues.
    /// For privacy reasons, the list of games controlled is not provided.
    /// </summary>
    public int OpenCloudIssues { get; set; } = 0;
}

[JsonSerializable(typeof(HealthCheckResponse))]
[JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true)]
public partial class HealthCheckResponseJsonContext : JsonSerializerContext
{
}