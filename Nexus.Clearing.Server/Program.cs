using Microsoft.AspNetCore.Diagnostics;
using Nexus.Clearing.Server;
using Nexus.Clearing.Server.Database;

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
        // Update the database.
        SqliteContext.PrepareDatabase();
        
        // Build the server.
        Logger.Debug("Preparing server.");
        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(Logger.NexusLogger);
        builder.Services.AddControllers();

        // Start the server.
        var app = builder.Build();
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionHandlerPathFeature != null)
                {
                    Logger.Error($"An exception occured processing {context.Request.Method} {context.Request.Path}\n{exceptionHandlerPathFeature.Error}");
                }
                return Task.CompletedTask;
            });
        });
        app.MapControllers();
        Logger.Info($"Starting server on port {Port}.");
        app.Run($"http://*:{Port}");
    }
}