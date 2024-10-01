using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nexus.Clearing.Server;
using Nexus.Clearing.Server.Controllers;
using Nexus.Clearing.Server.Database;
using Nexus.Clearing.Server.Model.Response;
using Nexus.Clearing.Server.Util;

public class Program
{
    /// <summary>
    /// Port for hosting the server.
    /// </summary>
    public const ushort Port = 8000;

    /// <summary>
    /// If true, ASP.NET logging will be active.
    /// </summary>
    public const bool EnableAspNetLogging = false;
    
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
        if (EnableAspNetLogging)
        {
            builder.Logging.AddProvider(Logger.NexusLogger);
        }
        builder.WebHost.UseKestrel(options => options.AddServerHeader = false);
        
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
        app.MapGet("/health", async (context) =>
        {
            var response = await healthController.PerformHealthCheckAsync();
            var statusCode = (response.Status == HealthCheckResultStatus.Down ? 503 : 200);
            await Results.Json(response, statusCode: statusCode, jsonTypeInfo: HealthCheckResponseJsonContext.Default.HealthCheckResponse).ExecuteAsync(context);
        });
        app.MapPost("/clearing/roblox", async (context) =>
        {
            var response = await clearingController.HandleRobloxWebhook(context);
            await Results.Text((string) response.Value!, statusCode: response.StatusCode).ExecuteAsync(context);
        });
        
        // Start the server.
        Logger.Info($"Starting server on port {Port}.");
        app.Run($"http://*:{Port}");
    }
}