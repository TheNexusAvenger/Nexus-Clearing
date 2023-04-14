using Nexus.Clearing.Server;

public class Program
{
    /// <summary>
    /// Port for hosting the server.
    /// </summary>
    public const ushort Port = 8000;
    
    /// <summary>
    /// Runs the program.
    /// </summary>
    /// <param name="args">Arguments from the command line.</param>
    public static void Main(string[] args)
    {
        // Build the server.
        Logger.Debug("Preparing server.");
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.ConfigureLogging(logging => logging.ClearProviders().AddProvider(Logger.NexusLogger));
        builder.Services.AddControllers();

        // Start the server.
        var app = builder.Build();
        app.MapControllers();
        Logger.Info($"Starting server on port {Port}.");
        app.Run($"http://*:{Port}");
    }
}