using Microsoft.AspNetCore.Diagnostics;
using Nexus.Clearing.Server;
using Nexus.Clearing.Server.Controllers;
using Nexus.Clearing.Server.Database;
using Nexus.Clearing.Server.Util;

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
        var builder = WebApplication.CreateSlimBuilder(args);
        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(Logger.NexusLogger);
        
        // Start background clearing.
        Task.Run(async () =>
        {
            while (true)
            {
                await ClearDataStores.ClearPendingUsersAsync();
                await Task.Delay(TimeSpan.FromMinutes(15));
            }
        });

        // Add exception handling.
        var app = builder.Build();
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionHandlerPathFeature != null)
                {
                    Logger.Error($"An exception occurred processing {context.Request.Method} {context.Request.Path}\n{exceptionHandlerPathFeature.Error}");
                }
                return Task.CompletedTask;
            });
        });
        
        // Register the routes.
        var healthController = new HealthController();
        var clearingController = new ClearingController();
        app.MapGet("/health", () => healthController.Get());
        app.MapPost("/clearing/roblox", async (context) => await clearingController.HandleRobloxWebhook(context));
        
        // Start the server.
        Logger.Info($"Starting server on port {Port}.");
        app.Run($"http://*:{Port}");
    }
}