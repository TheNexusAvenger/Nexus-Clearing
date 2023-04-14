using Microsoft.AspNetCore.Http;
using Nexus.Clearing.Server.Util;
using NUnit.Framework;

namespace Nexus.Clearing.Server.Test.Util;

public class VerificationTest
{
    /// <summary>
    /// Valid signature from Roblox used for tests.
    /// </summary>
    public const string ValidSignatureHeader = "t=1681441534,v1=paQDFIOugCVzRKuJPoN2l9alCyBySHgIkUpbMIKlWJY=";
    
    /// <summary>
    /// Valid secret used for tests.
    /// </summary>
    public const string ValidSecret = "test";
    
    /// <summary>
    /// Valid body from Roblox used for tests.
    /// </summary>
    public const string ValidBody = "{\"NotificationId\":\"64c4e627-52dd-4b69-a918-e5a6aec43c2e\",\"EventType\":\"SampleNotification\",\"EventTime\":\"2023-04-14T03:05:34.4767037Z\",\"EventPayload\":{\"UserId\":25691148}}";
    
    /// <summary>
    /// Tests VerifyRequest with a valid signature.
    /// </summary>
    [Test]
    public void TestVerifyRequest()
    {
        Assert.That(Verification.VerifyRequest(ValidSignatureHeader, ValidSecret, ValidBody), Is.EqualTo(true));
    }
    
    /// <summary>
    /// Tests VerifyRequest with an invalid signature due to the secret.
    /// </summary>
    [Test]
    public void TestVerifyRequestBadSecret()
    {
        Assert.That(Verification.VerifyRequest(ValidSignatureHeader, "tent", ValidBody), Is.EqualTo(false));
    }
    
    /// <summary>
    /// Tests VerifyRequest with an invalid signature due to the timestamp.
    /// </summary>
    [Test]
    public void TestVerifyRequestBadTimestamp()
    {
        Assert.That(Verification.VerifyRequest("t=1681441531,v1=paQDFIOugCVzRKuJPoN2l9alCyBySHgIkUpbMIKlWJY=", ValidSecret, ValidBody), Is.EqualTo(false));
    }
    
    /// <summary>
    /// Tests VerifyRequest with an invalid signature due to the body.
    /// </summary>
    [Test]
    public void TestVerifyRequestBadBody()
    {
        Assert.That(Verification.VerifyRequest(ValidSignatureHeader, ValidSecret, "{\"NotificationId\":\"74c4e627-52dd-4b69-a918-e5a6aec43c2e\",\"EventType\":\"SampleNotification\",\"EventTime\":\"2023-04-14T03:05:34.4767037Z\",\"EventPayload\":{\"UserId\":25691148}}"), Is.EqualTo(false));
    }
    
    /// <summary>
    /// Tests VerifyRequest with an invalid signature due to the signature.
    /// </summary>
    [Test]
    public void TestVerifyRequestBadSignature()
    {
        Assert.That(Verification.VerifyRequest("t=1681441534,v1=1aQDFIOugCVzRKuJPoN2l9alCyBySHgIkUpbMIKlWJY=", ValidSecret, ValidBody), Is.EqualTo(false));
    }
    
    /// <summary>
    /// Tests VerifyRequest with no signature.
    /// </summary>
    [Test]
    public void TestVerifyRequestNoSignature()
    {
        Assert.That(Verification.VerifyRequest("t=1681441534", ValidSecret, ValidBody), Is.EqualTo(false));
    }
    
    /// <summary>
    /// Tests VerifyRequest with an invalid timestamp header.
    /// </summary>
    [Test]
    public void TestVerifyRequestInvalidTimestampHeader()
    {
        Assert.That(Verification.VerifyRequest("t2=1681441534,v1=paQDFIOugCVzRKuJPoN2l9alCyBySHgIkUpbMIKlWJY=", ValidSecret, ValidBody), Is.EqualTo(false));
    }
    
    /// <summary>
    /// Tests VerifyRequest with a value in the header not set.
    /// </summary>
    [Test]
    public void TestVerifyRequestUnsetValueHeader()
    {
        Assert.That(Verification.VerifyRequest("t2=1681441534,v1", ValidSecret, ValidBody), Is.EqualTo(false));
    }
    
    /// <summary>
    /// Tests VerifyRequest with a HttpRequest.
    /// </summary>
    [Test]
    public void TestVerifyRequestHttpRequest()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Add("roblox-signature", ValidSignatureHeader);
        Assert.That(Verification.VerifyRequest(context.Request, ValidSecret, ValidBody), Is.EqualTo(true));
    }
    
    /// <summary>
    /// Tests VerifyRequest with a HttpRequest but with no request.
    /// </summary>
    [Test]
    public void TestVerifyRequestHttpRequestNoHeader()
    {
        var context = new DefaultHttpContext();
        Assert.That(Verification.VerifyRequest(context.Request, ValidSecret, ValidBody), Is.EqualTo(false));
    }
}