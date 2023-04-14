using Microsoft.EntityFrameworkCore;
using Nexus.Clearing.Server.Database.Model;

namespace Nexus.Clearing.Server.Database;

public class SqliteContext : DbContext, IAsyncDisposable
{
    /// <summary>
    /// DataStores to clear when requested.
    /// </summary>
    public DbSet<DataStore> DataStores { get; set; } = null!;
    
    /// <summary>
    /// Roblox game keys for the clearing.
    /// </summary>
    public DbSet<RobloxGameKey> RobloxGameKeys { get; set; } = null!;
    
    /// <summary>
    /// Roblox users to clear or have been cleared.
    /// </summary>
    public DbSet<RobloxUser> RobloxUsers { get; set; } = null!;
    
    /// <summary>
    /// Configures the context.
    /// </summary>
    /// <param name="optionsBuilder">Builder for options.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var location = Environment.GetEnvironmentVariable("DATABASE_LOCATION") ?? "database.sqlite";
        optionsBuilder.UseSqlite($"Data Source=\"{location}\"");
    }
    
    /// <summary>
    /// Ensures the database is up to date.
    /// </summary>
    public async Task EnsureUpdatedAsync()
    {
        await Database.MigrateAsync().ConfigureAwait(false);
    }
    
    /// <summary>
    /// Disposes of the context.
    /// </summary>
    public override ValueTask DisposeAsync()
    {
        return new ValueTask(Task.Run(Dispose));
    }

    /// <summary>
    /// Ensures the database is up to date.
    /// </summary>
    public static void PrepareDatabase()
    {
        Task.Run(async () =>
        {
            try
            {
                await using var context = new SqliteContext();
                await context.EnsureUpdatedAsync();
                Logger.Debug("Database is up to date.");
            }
            catch (Exception e)
            {
                Logger.Error($"Database failed to update.\n{e}");
            }
        }).Wait();
    }
}