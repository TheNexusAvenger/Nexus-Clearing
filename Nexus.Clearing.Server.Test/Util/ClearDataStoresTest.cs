using System.Linq;
using Nexus.Clearing.Server.Database;
using Nexus.Clearing.Server.Database.Model;
using Nexus.Clearing.Server.Enum;
using Nexus.Clearing.Server.Test.TestUtil;
using Nexus.Clearing.Server.Util;
using NUnit.Framework;

namespace Nexus.Clearing.Server.Test.Util;

public class ClearDataStoreTEst
{
    /// <summary>
    /// Communicator used for the tests.
    /// </summary>
    private TestRobloxOpenCloudCommunicator _communicator = null!;
    
    /// <summary>
    /// Sets up the test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        DatabaseUtil.ClearDatabase();
        this._communicator = new TestRobloxOpenCloudCommunicator();
        ClearDataStores.RobloxOpenCloudCommunicator = this._communicator;

        using var context = new SqliteContext();
        context.RobloxGameKeys.Add(new RobloxGameKey()
        {
            GameId = 1,
            WebHookSecret = "test",
            OpenCloudApiKey = "key1",
        });
        context.RobloxGameKeys.Add(new RobloxGameKey()
        {
            GameId = 2,
            WebHookSecret = "test",
            OpenCloudApiKey = "key2",
        });
        context.RobloxGameKeys.Add(new RobloxGameKey()
        {
            GameId = 3,
            WebHookSecret = "test",
        });
        context.DataStores.Add(new DataStore()
        {
            GameId = 1,
            DisplayName = "TestGame1",
            DataStoreName = "dataStore1",
            DataStoreKey = "dataStoreKey1",
        });
        context.DataStores.Add(new DataStore()
        {
            GameId = 1,
            DisplayName = "TestGame2",
            DataStoreName = "dataStore2",
            DataStoreKey = "dataStoreKey2_NoData",
        });
        context.DataStores.Add(new DataStore()
        {
            GameId = 1,
            DisplayName = "TestGame1",
            DataStoreName = "dataStore_{UserId}",
            DataStoreKey = "dataStoreKey_{UserId}",
        });
        context.DataStores.Add(new DataStore()
        {
            GameId = 3,
            DisplayName = "TestGame3",
            DataStoreName = "dataStore",
            DataStoreKey = "dataStoreKey",
        });
        context.SaveChanges();
    }

    /// <summary>
    /// Tests ClearPendingUserAsync.
    /// </summary>
    [Test]
    public void TestClearPendingUserAsync()
    {
        using var setupContext = new SqliteContext();
        setupContext.RobloxUsers.Add(new RobloxUser()
        {
            UserId = 123,
            GameIds = "1",
            Status = ClearingState.Pending,
        });
        setupContext.SaveChanges();

        ClearDataStores.ClearPendingUserAsync(123).Wait();
        using var context = new SqliteContext();
        Assert.That(this._communicator.Calls.Count, Is.EqualTo(5));
        Assert.That(this._communicator.Calls[0], Is.EqualTo((1, "dataStore1", "dataStoreKey1", TestRobloxOpenCloudCommunicator.OpenCloudCommunicatorCall.HasDataAsync)));
        Assert.That(this._communicator.Calls[1], Is.EqualTo((1, "dataStore1", "dataStoreKey1", TestRobloxOpenCloudCommunicator.OpenCloudCommunicatorCall.DeleteKeyAsync)));
        Assert.That(this._communicator.Calls[2], Is.EqualTo((1, "dataStore2", "dataStoreKey2_NoData", TestRobloxOpenCloudCommunicator.OpenCloudCommunicatorCall.HasDataAsync)));
        Assert.That(this._communicator.Calls[3], Is.EqualTo((1, "dataStore_123", "dataStoreKey_123", TestRobloxOpenCloudCommunicator.OpenCloudCommunicatorCall.HasDataAsync)));
        Assert.That(this._communicator.Calls[4], Is.EqualTo((1, "dataStore_123", "dataStoreKey_123", TestRobloxOpenCloudCommunicator.OpenCloudCommunicatorCall.DeleteKeyAsync)));
        Assert.That(context.RobloxUsers.First().Status, Is.EqualTo(ClearingState.Complete));
    }

    /// <summary>
    /// Tests ClearPendingUserAsync with no Data Store key.
    /// </summary>
    [Test]
    public void TestClearPendingUserAsyncNoDataStoreKeys()
    {
        using var setupContext = new SqliteContext();
        setupContext.RobloxUsers.Add(new RobloxUser()
        {
            UserId = 123,
            GameIds = "2",
            Status = ClearingState.Pending,
        });
        setupContext.SaveChanges();
        
        ClearDataStores.ClearPendingUserAsync(123).Wait();
        using var context = new SqliteContext();
        Assert.That(this._communicator.Calls.Count, Is.EqualTo(0));
        Assert.That(context.RobloxUsers.First().Status, Is.EqualTo(ClearingState.Complete));
    }

    /// <summary>
    /// Tests ClearPendingUserAsync with no Open Cloud key.
    /// </summary>
    [Test]
    public void TestClearPendingUserAsyncNoOpenCloudKey()
    {
        using var setupContext = new SqliteContext();
        setupContext.RobloxUsers.Add(new RobloxUser()
        {
            UserId = 123,
            GameIds = "3",
            Status = ClearingState.Pending,
        });
        setupContext.SaveChanges();
        
        ClearDataStores.ClearPendingUserAsync(123).Wait();
        using var context = new SqliteContext();
        Assert.That(this._communicator.Calls.Count, Is.EqualTo(0));
        Assert.That(context.RobloxUsers.First().Status, Is.EqualTo(ClearingState.AwaitingRetry));
    }

    /// <summary>
    /// Tests ClearPendingUserAsync with no key entry.
    /// </summary>
    [Test]
    public void TestClearPendingUserAsyncNoKey()
    {
        using var setupContext = new SqliteContext();
        setupContext.RobloxUsers.Add(new RobloxUser()
        {
            UserId = 123,
            GameIds = "4",
            Status = ClearingState.Pending,
        });
        setupContext.SaveChanges();
        
        ClearDataStores.ClearPendingUserAsync(123).Wait();
        using var context = new SqliteContext();
        Assert.That(this._communicator.Calls.Count, Is.EqualTo(0));
        Assert.That(context.RobloxUsers.First().Status, Is.EqualTo(ClearingState.Complete));
    }

    /// <summary>
    /// Tests ClearPendingUserAsync with pending users.
    /// </summary>
    [Test]
    public void TestClearPendingUsers()
    {
        using var setupContext = new SqliteContext();
        setupContext.RobloxUsers.Add(new RobloxUser()
        {
            UserId = 123,
            GameIds = "1",
            Status = ClearingState.Pending,
        });
        setupContext.RobloxUsers.Add(new RobloxUser()
        {
            UserId = 456,
            GameIds = "2",
            Status = ClearingState.Pending,
        });
        setupContext.SaveChanges();
        
        ClearDataStores.ClearPendingUsersAsync().Wait();
        using var context = new SqliteContext();
        Assert.That(this._communicator.Calls.Count, Is.EqualTo(5));
        Assert.That(this._communicator.Calls[0], Is.EqualTo((1, "dataStore1", "dataStoreKey1", TestRobloxOpenCloudCommunicator.OpenCloudCommunicatorCall.HasDataAsync)));
        Assert.That(this._communicator.Calls[1], Is.EqualTo((1, "dataStore1", "dataStoreKey1", TestRobloxOpenCloudCommunicator.OpenCloudCommunicatorCall.DeleteKeyAsync)));
        Assert.That(this._communicator.Calls[2], Is.EqualTo((1, "dataStore2", "dataStoreKey2_NoData", TestRobloxOpenCloudCommunicator.OpenCloudCommunicatorCall.HasDataAsync)));
        Assert.That(this._communicator.Calls[3], Is.EqualTo((1, "dataStore_123", "dataStoreKey_123", TestRobloxOpenCloudCommunicator.OpenCloudCommunicatorCall.HasDataAsync)));
        Assert.That(this._communicator.Calls[4], Is.EqualTo((1, "dataStore_123", "dataStoreKey_123", TestRobloxOpenCloudCommunicator.OpenCloudCommunicatorCall.DeleteKeyAsync)));
        Assert.That(context.RobloxUsers.First(user => user.UserId == 123).Status, Is.EqualTo(ClearingState.Complete));
        Assert.That(context.RobloxUsers.First(user => user.UserId == 456).Status, Is.EqualTo(ClearingState.Complete));
    }

    /// <summary>
    /// Tests ClearPendingUserAsync with cleared users.
    /// </summary>
    [Test] public void TestClearPendingUsersCleared()
    {
        using var setupContext = new SqliteContext();
        setupContext.RobloxUsers.Add(new RobloxUser()
        {
            UserId = 123,
            GameIds = "1",
            Status = ClearingState.Complete,
        });
        setupContext.RobloxUsers.Add(new RobloxUser()
        {
            UserId = 456,
            GameIds = "2",
            Status = ClearingState.Complete,
        });
        setupContext.SaveChanges();
        
        ClearDataStores.ClearPendingUsersAsync().Wait();
        using var context = new SqliteContext();
        Assert.That(this._communicator.Calls.Count, Is.EqualTo(0));
        Assert.That(context.RobloxUsers.First(user => user.UserId == 123).Status, Is.EqualTo(ClearingState.Complete));
        Assert.That(context.RobloxUsers.First(user => user.UserId == 456).Status, Is.EqualTo(ClearingState.Complete));
    }

    /// <summary>
    /// Tests ClearPendingUserAsync with an error.
    /// </summary>
    [Test] public void TestClearPendingUsersError()
    {
        using var setupContext = new SqliteContext();
        setupContext.RobloxUsers.Add(new RobloxUser()
        {
            UserId = 456,
            GameIds = "3",
            Status = ClearingState.Pending,
        });
        setupContext.RobloxUsers.Add(new RobloxUser()
        {
            UserId = 123,
            GameIds = "1",
            Status = ClearingState.Pending,
        });
        setupContext.SaveChanges();
        
        ClearDataStores.ClearPendingUsersAsync().Wait();
        using var context = new SqliteContext();
        Assert.That(this._communicator.Calls.Count, Is.EqualTo(5));
        Assert.That(this._communicator.Calls[0], Is.EqualTo((1, "dataStore1", "dataStoreKey1", TestRobloxOpenCloudCommunicator.OpenCloudCommunicatorCall.HasDataAsync)));
        Assert.That(this._communicator.Calls[1], Is.EqualTo((1, "dataStore1", "dataStoreKey1", TestRobloxOpenCloudCommunicator.OpenCloudCommunicatorCall.DeleteKeyAsync)));
        Assert.That(this._communicator.Calls[2], Is.EqualTo((1, "dataStore2", "dataStoreKey2_NoData", TestRobloxOpenCloudCommunicator.OpenCloudCommunicatorCall.HasDataAsync)));
        Assert.That(this._communicator.Calls[3], Is.EqualTo((1, "dataStore_123", "dataStoreKey_123", TestRobloxOpenCloudCommunicator.OpenCloudCommunicatorCall.HasDataAsync)));
        Assert.That(this._communicator.Calls[4], Is.EqualTo((1, "dataStore_123", "dataStoreKey_123", TestRobloxOpenCloudCommunicator.OpenCloudCommunicatorCall.DeleteKeyAsync)));
        Assert.That(context.RobloxUsers.First(user => user.UserId == 123).Status, Is.EqualTo(ClearingState.Complete));
        Assert.That(context.RobloxUsers.First(user => user.UserId == 456).Status, Is.EqualTo(ClearingState.AwaitingRetry));
    }
}