using NUnit.Framework;
using TestAutomation.Challenges.API.Base;

namespace TestAutomation.Challenges.API;

/// <summary>
/// Framework smoke tests for the API project.
///
/// These tests are NOT challenges — they are fully implemented and must
/// always remain green. Their sole purpose is to prove that the framework
/// plumbing is correctly wired before any candidate code is written:
///
///   • <see cref="ApiClient"/> is constructed and injected by <see cref="BaseApiTest"/>.
///   • The HttpClient base address resolves to the ReqRes base URL.
///   • A real HTTP GET succeeds and returns an expected status code.
///   • TestSettings resolves the ReqRes base URL without error.
///
/// If any of these tests fail, fix the framework before attempting challenges.
/// </summary>
[TestFixture]
[Category("Smoke")]
public sealed class FrameworkSmokeTests : BaseApiTest
{
    /// <summary>
    /// Verifies that the ApiClient can reach the ReqRes API and that
    /// GET /api/users returns HTTP 200.
    /// A passing result confirms the full API framework stack is operational.
    /// </summary>
    [Test]
    public async Task API_Framework_ClientCanReachReqResAndReceive200()
    {
        var response = await Client.GetAsync("/api/users?page=1");

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK),
            "Expected GET /api/users to return HTTP 200 OK");
    }

    /// <summary>
    /// Verifies that the response body from GET /api/users is not empty,
    /// confirming that JSON is being returned and the client can read it.
    /// </summary>
    [Test]
    public async Task API_Framework_ResponseBodyIsNotEmpty()
    {
        var response = await Client.GetAsync("/api/users?page=1");
        var body     = await response.Content.ReadAsStringAsync();

        Assert.That(body, Is.Not.Null.And.Not.Empty,
            "Expected a non-empty JSON body from GET /api/users");
    }

    /// <summary>
    /// Verifies that TestSettings resolves the ReqRes base URL
    /// and that the value is a well-formed URI.
    /// </summary>
    [Test]
    public void API_Framework_TestSettings_ResolvesReqResBaseUrl()
    {
        Assert.That(Settings.ReqResBaseUrl, Is.Not.Null.And.Not.Empty,
            "ReqResBaseUrl must be set");

        Assert.That(Uri.IsWellFormedUriString(Settings.ReqResBaseUrl, UriKind.Absolute), Is.True,
            $"ReqResBaseUrl '{Settings.ReqResBaseUrl}' must be a well-formed absolute URI");
    }
}
