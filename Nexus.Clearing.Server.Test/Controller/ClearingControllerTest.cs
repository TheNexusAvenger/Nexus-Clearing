using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexus.Clearing.Server.Controllers;
using Nexus.Clearing.Server.Database;
using Nexus.Clearing.Server.Database.Model;
using Nexus.Clearing.Server.Enum;
using Nexus.Clearing.Server.Test.TestUtil;
using NUnit.Framework;

namespace Nexus.Clearing.Server.Test.Controller;

public class ClearingControllerTest
{
    /// <summary>
    /// Clearing controller under test.
    /// </summary>
    private ClearingController _clearingController = null!;

    /// <summary>
    /// Sets the body of the test request.
    /// </summary>
    /// <param name="body">Body to set.</param>
    private void SetRequest(string body)
    {
        using var sha256 = new HMACSHA256(Encoding.UTF8.GetBytes("test"));
        var newSignature = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes($"1681441534.{body}")));
        this._clearingController.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext(),
        };
        this._clearingController.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        this._clearingController.Request.Headers.Add("roblox-signature", $"t=1681441534,v1={newSignature}");
    }
    
    /// <summary>
    /// Sets up the test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        DatabaseUtil.ClearDatabase();
        this._clearingController = new ClearingController();
        this.SetRequest("{\"NotificationId\":\"64c4e627-52dd-4b69-a918-e5a6aec43c2e\",\"EventType\":\"RightToErasureRequest\",\"EventTime\":\"2023-04-14T03:05:34.4767037Z\",\"EventPayload\":{\"UserId\":12345,\"GameIds\":[123,456]}}");

        using var context = new SqliteContext();
        context.RobloxGameKeys.Add(new RobloxGameKey()
        {
            GameId = 123,
            WebHookSecret = "test",
        });
        context.SaveChanges();
    }
    
    /// <summary>
    /// Tests HandleRobloxWebhook with a valid request.
    /// </summary>
    [Test]
    public void TestHandleRobloxWebhook()
    {
        var response = this._clearingController.HandleRobloxWebhook().Result;
        Assert.That(response.StatusCode, Is.EqualTo(200));
        Assert.That(response.Value, Is.EqualTo("Success"));
        using var context = new SqliteContext();
        Assert.That(context.RobloxUsers.Count(), Is.EqualTo(1));
        Assert.That(context.RobloxUsers.First().UserId, Is.EqualTo(12345));
        Assert.That(context.RobloxUsers.First().GameIds, Is.EqualTo("123,456"));
    }
    
    /// <summary>
    /// Tests HandleRobloxWebhook with an invalid event type.
    /// </summary>
    [Test]
    public void TestHandleRobloxWebhookInvalidEventType()
    {
        this.SetRequest("{\"NotificationId\":\"64c4e627-52dd-4b69-a918-e5a6aec43c2e\",\"EventType\":\"SampleNotification\",\"EventTime\":\"2023-04-14T03:05:34.4767037Z\",\"EventPayload\":{\"UserId\":12345}}");
        var response = this._clearingController.HandleRobloxWebhook().Result;
        Assert.That(response.StatusCode, Is.EqualTo(400));
        Assert.That(response.Value, Is.EqualTo("InvalidEventType"));
        using var context = new SqliteContext();
        Assert.That(context.RobloxUsers.Count(), Is.EqualTo(0));
    }
    
    /// <summary>
    /// Tests HandleRobloxWebhook with an invalid signature.
    /// </summary>
    [Test]
    public void TestHandleRobloxWebhookInvalidSignature()
    {
        using var setupContext = new SqliteContext();
        setupContext.RobloxGameKeys.First().WebHookSecret = "unknown";
        setupContext.SaveChanges();
        
        var response = this._clearingController.HandleRobloxWebhook().Result;
        using var context = new SqliteContext();
        Assert.That(response.StatusCode, Is.EqualTo(401));
        Assert.That(response.Value, Is.EqualTo("InvalidSignature"));
        Assert.That(context.RobloxUsers.Count(), Is.EqualTo(0));
    }
    
    /// <summary>
    /// Tests HandleRobloxWebhook with an already queued user.
    /// </summary>
    [Test]
    public void TestHandleRobloxWebhookQueuedUser()
    {
        using var setupContext = new SqliteContext();
        setupContext.RobloxUsers.Add(new RobloxUser()
        {
            UserId = 12345,
            GameIds = "123,789",
            Status = ClearingState.Pending,
        });
        setupContext.SaveChanges();
        
        var response = this._clearingController.HandleRobloxWebhook().Result;
        using var context = new SqliteContext();
        Assert.That(response.StatusCode, Is.EqualTo(200));
        Assert.That(response.Value, Is.EqualTo("AlreadyQueued"));
        Assert.That(context.RobloxUsers.Count(), Is.EqualTo(1));
    }
    
    /// <summary>
    /// Tests HandleRobloxWebhook with an already cleared user.
    /// </summary>
    [Test]
    public void TestHandleRobloxWebhookClearedUser()
    {
        using var setupContext = new SqliteContext();
        setupContext.RobloxUsers.Add(new RobloxUser()
        {
            UserId = 12345,
            GameIds = "123,789",
            Status = ClearingState.Complete,
        });
        setupContext.SaveChanges();
        
        var response = this._clearingController.HandleRobloxWebhook().Result;
        using var context = new SqliteContext();
        Assert.That(response.StatusCode, Is.EqualTo(200));
        Assert.That(response.Value, Is.EqualTo("Success"));
        Assert.That(context.RobloxUsers.Count(), Is.EqualTo(1));
        Assert.That(context.RobloxUsers.First().GameIds, Is.EqualTo("123,789,456"));
        Assert.That(context.RobloxUsers.First().Status, Is.EqualTo(ClearingState.Pending));
    }
}