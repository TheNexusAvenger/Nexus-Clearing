using Nexus.Clearing.Server.Controllers;
using Nexus.Clearing.Server.Database;
using Nexus.Clearing.Server.Database.Model;
using Nexus.Clearing.Server.Model.Response;
using Nexus.Clearing.Server.Test.TestUtil;
using NUnit.Framework;

namespace Nexus.Clearing.Server.Test.Controller;

public class HealthControllerTest
{
    private HealthController _healthController = null!;
    private TestRobloxOpenCloudCommunicator _robloxOpenCloudCommunicator = null!;
    
    [SetUp]
    public void SetUp()
    {
        DatabaseUtil.ClearDatabase();
        this._robloxOpenCloudCommunicator = new TestRobloxOpenCloudCommunicator();
        this._healthController = new HealthController(this._robloxOpenCloudCommunicator);

        using var context = new SqliteContext();
        context.DataStores.Add(new DataStore()
        {
            GameId = 123,
            DisplayName = "TestName",
            DataStoreName = "TestDataStore",
            DataStoreKey = "TestKey",
        });
        context.SaveChanges();
    }

    [Test]
    public void TestPerformHealthCheckAsyncMissingApiKey()
    {
        var response = this._healthController.PerformHealthCheckAsync().Result;
        Assert.That(response.Status, Is.EqualTo(HealthCheckResultStatus.Down));
        Assert.That(response.OpenCloudIssues, Is.EqualTo(1));
    }

    [Test]
    public void TestPerformHealthCheckAsyncError()
    {
        this._robloxOpenCloudCommunicator.ThrowExceptionOnCall = true;
        using var context = new SqliteContext();
        context.RobloxGameKeys.Add(new RobloxGameKey()
        {
            GameId = 123,
            WebHookSecret = "TestSecret",
            OpenCloudApiKey = "TestApiKey",
        });
        context.SaveChanges();
        
        var response = this._healthController.PerformHealthCheckAsync().Result;
        Assert.That(response.Status, Is.EqualTo(HealthCheckResultStatus.Down));
        Assert.That(response.OpenCloudIssues, Is.EqualTo(1));
    }

    [Test]
    public void TestPerformHealthCheckAsyncSuccess()
    {
        using var context = new SqliteContext();
        context.RobloxGameKeys.Add(new RobloxGameKey()
        {
            GameId = 123,
            WebHookSecret = "TestSecret",
            OpenCloudApiKey = "TestApiKey",
        });
        context.SaveChanges();
        
        var response = this._healthController.PerformHealthCheckAsync().Result;
        Assert.That(response.Status, Is.EqualTo(HealthCheckResultStatus.Up));
        Assert.That(response.OpenCloudIssues, Is.EqualTo(0));
    }
}