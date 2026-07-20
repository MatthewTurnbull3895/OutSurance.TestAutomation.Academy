using NUnit.Framework;
using TestAutomation.Challenges.API.Base;
using TestAutomation.Challenges.API.ReqRes.Models;
using TestAutomation.Core.Http;

namespace TestAutomation.Challenges.API.ReqRes;

/// <summary>
/// 🟡 CHALLENGE 4 — ReqRes Authentication API (Intermediate → Advanced)
///
/// Objective:
///   Verify the login and registration endpoints exposed by https://reqres.in/api.
///
/// Skills practised:
///   • Asserting on both success and error response bodies
///   • Understanding HTTP 400 Bad Request vs 200 OK flows
///   • Extracting and validating tokens from response payloads
///   • Writing parameterised negative tests
///
/// Setup:
///   <see cref="BaseApiTest.Client"/> is pre-configured with the ReqRes base URL
///   and the required API key — no additional setup is needed.
///   ReqRes only accepts specific pre-seeded email addresses.
///
///   Valid credentials (these are public test credentials for reqres.in):
///     Login:    email "eve.holt@reqres.in",  password "cityslicka"
///     Register: email "eve.holt@reqres.in",  password "pistol"
///
///   To deserialise a JSON response body into a typed object use:
///   <code>
///   var body = await ApiClient.DeserialiseAsync&lt;MyResponseType&gt;(response);
///   </code>
///
/// Acceptance criteria are stated in each test's XML doc comment.
/// Implement ONLY the method body — do not change method signatures or attributes.
/// </summary>
[TestFixture]
[Category("ReqRes")]
[Category("Auth")]
public sealed class Challenge4_AuthApiTests : BaseApiTest
{
    // -------------------------------------------------------------------------
    // 🟢 Beginner tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Test: POST /api/login with valid credentials returns HTTP 200 and a token.
    ///
    /// Steps:
    ///   1. Send POST /api/login with body: { "email": "eve.holt@reqres.in", "password": "cityslicka" }.
    ///
    /// Acceptance criteria:
    ///   • Status code is 200 OK.
    ///   • The response body contains a "token" field that is not null or empty.
    /// </summary>
    [Test]
    [Description("🟢 Verify POST /api/login with valid credentials returns 200 and a non-empty token.")]
    public async Task Login_ValidCredentials_ShouldReturn200_WithToken()
    {
        var payload = new { email = "eve.holt@reqres.in", password = "cityslicka" };
        var response = await Client.PostAsync("/api/login", payload);
        var body = await ApiClient.DeserialiseAsync<TokenResponse>(response);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK),
            "Expected 200 OK for valid login");
        Assert.That(body?.Token, Is.Not.Null.And.Not.Empty,
            "Expected a non-empty token in the response");
    }

    /// <summary>
    /// Test: POST /api/login with a missing password returns HTTP 400 and an error.
    ///
    /// Steps:
    ///   1. Send POST /api/login with body: { "email": "eve.holt@reqres.in" } (no password).
    ///
    /// Acceptance criteria:
    ///   • Status code is 400 Bad Request.
    ///   • The response body contains an "error" field with the value "Missing password".
    /// </summary>
    [Test]
    [Description("🟢 Verify POST /api/login without a password returns 400 with an error message.")]
    public async Task Login_MissingPassword_ShouldReturn400_WithErrorMessage()
    {
        var payload = new { email = "eve.holt@reqres.in" };
        var response = await Client.PostAsync("/api/login", payload);
        var body = await ApiClient.DeserialiseAsync<ErrorResponse>(response);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest),
            "Expected 400 Bad Request when password is missing");
        Assert.That(body?.Error, Is.EqualTo("Missing password"),
            "Expected error message to say 'Missing password'");
    }

    // -------------------------------------------------------------------------
    // 🟡 Intermediate tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Test: POST /api/register with valid credentials returns HTTP 200, an id and a token.
    ///
    /// Steps:
    ///   1. Send POST /api/register with body: { "email": "eve.holt@reqres.in", "password": "pistol" }.
    ///
    /// Acceptance criteria:
    ///   • Status code is 200 OK.
    ///   • The response body contains an "id" field with a value greater than 0.
    ///   • The response body contains a "token" field that is not null or empty.
    /// </summary>
    [Test]
    [Description("🟡 Verify POST /api/register with valid credentials returns 200, an id and a token.")]
    public async Task Register_ValidCredentials_ShouldReturn200_WithIdAndToken()
    {
        var payload = new { email = "eve.holt@reqres.in", password = "pistol" };
        var response = await Client.PostAsync("/api/register", payload);
        var body = await ApiClient.DeserialiseAsync<RegisterResponse>(response);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK),
            "Expected 200 OK for valid registration");
        Assert.That(body?.Id, Is.GreaterThan(0),
            "Expected a non-zero id in the response");
        Assert.That(body?.Token, Is.Not.Null.And.Not.Empty,
            "Expected a non-empty token in the response");
    }

    /// <summary>
    /// Test: POST /api/register with a missing password returns HTTP 400 and an error.
    ///
    /// Steps:
    ///   1. Send POST /api/register with body: { "email": "eve.holt@reqres.in" } (no password).
    ///
    /// Acceptance criteria:
    ///   • Status code is 400 Bad Request.
    ///   • The response body contains an "error" field with the value "Missing password".
    /// </summary>
    [Test]
    [Description("🟡 Verify POST /api/register without a password returns 400 with an error message.")]
    public async Task Register_MissingPassword_ShouldReturn400_WithErrorMessage()
    {
        var payload = new { email = "eve.holt@reqres.in" };
        var response = await Client.PostAsync("/api/register", payload);
        var body = await ApiClient.DeserialiseAsync<ErrorResponse>(response);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest),
            "Expected 400 Bad Request when password is missing");
        Assert.That(body?.Error, Is.EqualTo("Missing password"),
            "Expected error message to say 'Missing password'");
    }

    // -------------------------------------------------------------------------
    // 🟠 Advanced tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Test: POST /api/login with invalid or unrecognised credentials returns HTTP 400.
    ///
    /// Steps:
    ///   1. For each invalid payload supplied by the test case source, send POST /api/login.
    ///
    /// Acceptance criteria:
    ///   • Every request returns 400 Bad Request.
    ///   • Every response body contains a non-empty "error" field.
    ///
    /// Hint: The invalid payloads are already provided via [TestCaseSource] below —
    ///       use the <c>email</c> and <c>password</c> parameters to build each request body.
    ///       Note: <c>password</c> may be <c>null</c> — handle this case in your payload construction.
    /// </summary>
    [Test]
    [TestCaseSource(nameof(InvalidLoginPayloads))]
    [Description("🟠 Verify that various malformed login payloads all return 400 with an error.")]
    public async Task Login_InvalidPayloads_ShouldAllReturn400_WithError(string email, string? password)
    {
        var payload = password is null
            ? (object)new { email }
            : new { email, password };

        var response = await Client.PostAsync("/api/login", payload);
        var body = await ApiClient.DeserialiseAsync<ErrorResponse>(response);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest),
            $"Expected 400 Bad Request for email='{email}', password='{password}'");
        Assert.That(body?.Error, Is.Not.Null.And.Not.Empty,
            "Expected a non-empty error field in the response");
    }

    private static IEnumerable<object?[]> InvalidLoginPayloads() =>
    [
        ["not-an-email@fake.com", "somepassword"],
        ["eve.holt@reqres.in",    null],
        ["",                      "cityslicka"]
    ];
}
