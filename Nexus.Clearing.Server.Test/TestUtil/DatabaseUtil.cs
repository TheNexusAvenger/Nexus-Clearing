using Nexus.Clearing.Server.Database;

namespace Nexus.Clearing.Server.Test.TestUtil;

public static class DatabaseUtil
{
    /// <summary>
    /// Clears the database for running tests.
    /// </summary>
    public static void ClearDatabase()
    {
        SqliteContext.PrepareDatabase();
        using var context = new SqliteContext();
        context.DataStores.RemoveRange(context.DataStores);
        context.RobloxGameKeys.RemoveRange(context.RobloxGameKeys);
        context.RobloxUsers.RemoveRange(context.RobloxUsers);
        context.SaveChanges();
    }
}