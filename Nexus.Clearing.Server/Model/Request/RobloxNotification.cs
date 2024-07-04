using System.Text.Json.Serialization;

namespace Nexus.Clearing.Server.Model.Request;

public class RobloxNotification<T> where T : class
{
    /// <summary>
    /// Id of the notification.
    /// </summary>
    public string NotificationId { get; set; } = null!;

    /// <summary>
    /// Type of the event.
    /// </summary>
    public string EventType { get; set; } = null!;

    /// <summary>
    /// Time of the event.
    /// </summary>
    public DateTime EventTime { get; set; }

    /// <summary>
    /// Payload of the event.
    /// </summary>
    public T EventPayload { get; set; } = null!;
}

[JsonSerializable(typeof(RobloxNotification<RightToErasureRequestEventPayload>), TypeInfoPropertyName = "RobloxNotificationRightToErasureRequestEventPayload")]
[JsonSerializable(typeof(RightToErasureRequestEventPayload), TypeInfoPropertyName = "RightToErasureRequestEventPayload")]
internal partial class RobloxNotificationJsonContext : JsonSerializerContext
{
}