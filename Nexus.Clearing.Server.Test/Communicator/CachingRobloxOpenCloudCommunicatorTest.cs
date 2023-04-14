using Nexus.Clearing.Server.Communicator.Roblox;
using Nexus.Clearing.Server.Test.TestUtil;
using NUnit.Framework;

namespace Nexus.Clearing.Server.Test.Communicator;

public class CachingRobloxOpenCloudCommunicatorTest
{
    /// <summary>
    /// Tests GetGameIdFromPlaceIdAsync with caching.
    /// </summary>
    [Test]
    public void TestGetGameIdFromPlaceIdAsync()
    {
        var cachingCommunicator = new CachingRobloxOpenCloudCommunicator();
        var testCommunicator = new TestRobloxOpenCloudCommunicator();
        cachingCommunicator.Communicator = testCommunicator;
        
        Assert.That(cachingCommunicator.GetGameIdFromPlaceIdAsync(10).Result, Is.EqualTo(1));
        Assert.That(testCommunicator.Calls.Count, Is.EqualTo(1));
        Assert.That(cachingCommunicator.GetGameIdFromPlaceIdAsync(10).Result, Is.EqualTo(1));
        Assert.That(testCommunicator.Calls.Count, Is.EqualTo(1));
        
        Assert.That(cachingCommunicator.GetGameIdFromPlaceIdAsync(1).Result, Is.Null);
        Assert.That(testCommunicator.Calls.Count, Is.EqualTo(2));
        Assert.That(cachingCommunicator.GetGameIdFromPlaceIdAsync(1).Result, Is.Null);
        Assert.That(testCommunicator.Calls.Count, Is.EqualTo(2));
    }
}