using Nexus.Logging.Attribute;
using Nexus.Logging.Output;

namespace Nexus.Clearing.Server;

public class Logger
{
    /// <summary>
    /// Static instance of the logger.
    /// </summary>
    public static readonly Nexus.Logging.Logger NexusLogger = new Nexus.Logging.Logger();

    /// <summary>
    /// Sets up the logging.
    /// </summary>
    static Logger()
    {
        NexusLogger.Outputs.Add(new ConsoleOutput()
        {
            IncludeDate = true,
            NamespaceWhitelist = new List<string>() { "Nexus.Clearing" },
            MinimumLevel = LogLevel.Debug,
            DefaultLineWidth = 200,
        });
    }

    /// <summary>
    /// Logs a message as a Debug level.
    /// </summary>
    /// <param name="content">Content to log. Can be an object, like an exception.</param>
    [LogTraceIgnore]
    public static void Debug(object content)
    {
        NexusLogger.Debug(content);
    }

    /// <summary>
    /// Logs a message as an Information level.
    /// </summary>
    /// <param name="content">Content to log. Can be an object, like an exception.</param>
    [LogTraceIgnore]
    public static void Info(object content)
    {
        NexusLogger.Info(content);
    }

    /// <summary>
    /// Logs a message as a Warning level.
    /// </summary>
    /// <param name="content">Content to log. Can be an object, like an exception.</param>
    [LogTraceIgnore]
    public static void Warn(object content)
    {
        NexusLogger.Warn(content);
    }

    /// <summary>
    /// Logs a message as a Error level.
    /// </summary>
    /// <param name="content">Content to log. Can be an object, like an exception.</param>
    [LogTraceIgnore]
    public static void Error(object content)
    {
        NexusLogger.Error(content);
    }
}