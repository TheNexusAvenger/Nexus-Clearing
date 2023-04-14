using System.Collections.Generic;
using Nexus.Clearing.Server.Database.Model;
using NUnit.Framework;

namespace Nexus.Clearing.Server.Test.Database.Model;

public class RobloxUserTest
{
    /// <summary>
    /// Tests GetGameIds.
    /// </summary>
    [Test]
    public void TestGetGameIds()
    {
        var user = new RobloxUser()
        {
            GameIds = "123,456,789",
        };
        Assert.That(user.GetGameIds(), Is.EqualTo(new List<long>() {123, 456, 789}));
    }
    
    /// <summary>
    /// Tests GetGameIds with an empty string.
    /// </summary>
    [Test]
    public void TestGetGameIdsEmptyString()
    {
        var user = new RobloxUser()
        {
            GameIds = "",
        };
        Assert.That(user.GetGameIds(), Is.EqualTo(new List<long>()));
    }
    
    /// <summary>
    /// Tests SetGameIds.
    /// </summary>
    [Test]
    public void TestSetGameIds()
    {
        var user = new RobloxUser();
        user.SetGameIds(new List<long>() {123, 456, 789});
        Assert.That(user.GameIds, Is.EqualTo("123,456,789"));
    }
}