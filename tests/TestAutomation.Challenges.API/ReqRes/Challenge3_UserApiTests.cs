using NUnit.Framework;
using TestAutomation.Challenges.API.Base;
using TestAutomation.Challenges.API.ReqRes.Models;
using TestAutomation.Core.Http;

namespace TestAutomation.Challenges.API.ReqRes;

/// <summary>
/// 🟢 CHALLENGE 3 — ReqRes Users API (Beginner → Intermediate)
///
/// Objective:
///   Verify the core user-management endpoints exposed by https://reqres.in/api/users.
///
/// Skills practised:
///   • Sending GET, POST, PUT, PATCH and DELETE requests via <see cref="ApiClient"/>
///   • Asserting on HTTP status codes
///   • Deserialising and inspecting JSON response bodies
///   • Data-driven API tests with [TestCaseSource]
///
/// Setup:
///   <see cref="BaseApiTest.Client"/> is pre-configured with the ReqRes base URL
///   and the required API key — no additional setup is needed.
///   All endpoints are relative, e.g. "/api/users" or "/api/users/2".
///
///   To deserialise a JSON response body into a typed object use:
///   <code>
///   var body = await ApiClient.DeserialiseAsync&lt;MyResponseType&gt;(response);
///   </code>
///   You are free to define your own response model classes, or use
///   anonymous types / <c>JsonDocument</c> for simpler assertions.
///
/// Acceptance criteria are stated in each test's XML doc comment.
/// Implement ONLY the method body — do not change method signatures or attributes.
/// </summary>
[TestFixture]
[Category("ReqRes")]
[Category("Users")]
public sealed class Challenge3_UserApiTests : BaseApiTest
{
    // -------------------------------------------------------------------------
    // 🟢 Beginner tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Test: GET /api/users returns HTTP 200 and a non-empty user list.
    ///
    /// Steps:
    ///   1. Send GET /api/users?page=1.
    ///
    /// Acceptance criteria:
    ///   • Status code is 200 OK.
    ///   • The response body contains a "data" array with at least one user.
    ///   • Each user object has a non-zero "id" field.
    /// </summary>
    [Test]
    [Description("🟢 Verify GET /api/users returns 200 and a populated user list.")]
    public async Task GetUsers_ShouldReturn200_WithNonEmptyList()
    {
        var response = await Client.GetAsync("/api/users?page=1");
        var body = await ApiClient.DeserialiseAsync<UserListResponse>(response);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK),
            "Expected 200 OK");
        Assert.That(body?.Data, Is.Not.Null.And.Not.Empty,
            "Expected a non-empty data array");
        Assert.That(body!.Data.All(u => u.Id > 0), Is.True,
            "Expected every user to have a non-zero id");
    }

    /// <summary>
    /// Test: GET /api/users/{id} returns the correct single user.
    ///
    /// Steps:
    ///   1. Send GET /api/users/2.
    ///
    /// Acceptance criteria:
    ///   • Status code is 200 OK.
    ///   • The response body contains a "data" object.
    ///   • The "id" field equals 2.
    ///   • The "email" field is not null or empty.
    /// </summary>
    [Test]
    [Description("🟢 Verify GET /api/users/2 returns 200 with the correct user data.")]
    public async Task GetUserById_ExistingUser_ShouldReturn200_WithCorrectData()
    {
        var response = await Client.GetAsync("/api/users/2");
        var body = await ApiClient.DeserialiseAsync<SingleUserResponse>(response);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK),
            "Expected 200 OK");
        Assert.That(body?.Data, Is.Not.Null,
            "Expected a data object in the response");
        Assert.That(body!.Data.Id, Is.EqualTo(2),
            "Expected user id to equal 2");
        Assert.That(body.Data.Email, Is.Not.Null.And.Not.Empty,
            "Expected email to be populated");
    }

    /// <summary>
    /// Test: GET /api/users/{id} for a non-existent user returns HTTP 404.
    ///
    /// Steps:
    ///   1. Send GET /api/users/9999.
    ///
    /// Acceptance criteria:
    ///   • Status code is 404 Not Found.
    /// </summary>
    [Test]
    [Description("🟢 Verify GET /api/users/9999 returns 404 for a user that does not exist.")]
    public async Task GetUserById_NonExistentUser_ShouldReturn404()
    {
        var response = await Client.GetAsync("/api/users/9999");

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound),
            "Expected 404 Not Found for a non-existent user");
    }

    // -------------------------------------------------------------------------
    // 🟡 Intermediate tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Test: POST /api/users creates a new user and returns HTTP 201.
    ///
    /// Steps:
    ///   1. Send POST /api/users with a JSON body containing "name" and "job" fields.
    ///
    /// Acceptance criteria:
    ///   • Status code is 201 Created.
    ///   • The response body contains an "id" field that is not null or empty.
    ///   • The response body contains a "createdAt" timestamp field.
    ///   • The "name" and "job" values in the response match what was sent.
    /// </summary>
    [Test]
    [Description("🟡 Verify POST /api/users returns 201 and echoes back the created user data.")]
    public async Task CreateUser_ValidPayload_ShouldReturn201_WithCreatedUser()
    {
        var payload = new { name = "Academy Tester", job = "QA Engineer" };
        var response = await Client.PostAsync("/api/users", payload);
        var body = await ApiClient.DeserialiseAsync<CreateUserResponse>(response);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Created),
            "Expected 201 Created");
        Assert.That(body?.Id, Is.Not.Null.And.Not.Empty,
            "Expected a non-empty id in the response");
        Assert.That(body?.CreatedAt, Is.Not.Null.And.Not.Empty,
            "Expected a createdAt timestamp in the response");
        Assert.That(body?.Name, Is.EqualTo("Academy Tester"),
            "Expected name to match the payload");
        Assert.That(body?.Job, Is.EqualTo("QA Engineer"),
            "Expected job to match the payload");
    }

    /// <summary>
    /// Test: PUT /api/users/{id} fully updates a user and returns HTTP 200.
    ///
    /// Steps:
    ///   1. Send PUT /api/users/2 with a JSON body containing "name" and "job" fields.
    ///
    /// Acceptance criteria:
    ///   • Status code is 200 OK.
    ///   • The response body reflects the new "name" and "job" values.
    ///   • The response body contains an "updatedAt" timestamp field.
    /// </summary>
    [Test]
    [Description("🟡 Verify PUT /api/users/2 returns 200 and reflects the updated values.")]
    public async Task UpdateUser_WithPut_ShouldReturn200_WithUpdatedValues()
    {
        var payload = new { name = "Updated Tester", job = "Senior QA" };
        var response = await Client.PutAsync("/api/users/2", payload);
        var body = await ApiClient.DeserialiseAsync<UpdateUserResponse>(response);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK),
            "Expected 200 OK");
        Assert.That(body?.Name, Is.EqualTo("Updated Tester"),
            "Expected name to reflect the updated value");
        Assert.That(body?.Job, Is.EqualTo("Senior QA"),
            "Expected job to reflect the updated value");
        Assert.That(body?.UpdatedAt, Is.Not.Null.And.Not.Empty,
            "Expected an updatedAt timestamp in the response");
    }

    /// <summary>
    /// Test: PATCH /api/users/{id} partially updates a user and returns HTTP 200.
    ///
    /// Steps:
    ///   1. Send PATCH /api/users/2 with a JSON body containing only the "job" field.
    ///
    /// Acceptance criteria:
    ///   • Status code is 200 OK.
    ///   • The response body reflects the new "job" value.
    ///   • The response body contains an "updatedAt" timestamp field.
    /// </summary>
    [Test]
    [Description("🟡 Verify PATCH /api/users/2 returns 200 and reflects only the patched field.")]
    public async Task UpdateUser_WithPatch_ShouldReturn200_WithPatchedValue()
    {
        var payload = new { job = "Automation Lead" };
        var response = await Client.PatchAsync("/api/users/2", payload);
        var body = await ApiClient.DeserialiseAsync<UpdateUserResponse>(response);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK),
            "Expected 200 OK");
        Assert.That(body?.Job, Is.EqualTo("Automation Lead"),
            "Expected job to reflect the patched value");
        Assert.That(body?.UpdatedAt, Is.Not.Null.And.Not.Empty,
            "Expected an updatedAt timestamp in the response");
    }

    /// <summary>
    /// Test: DELETE /api/users/{id} returns HTTP 204 with no response body.
    ///
    /// Steps:
    ///   1. Send DELETE /api/users/2.
    ///
    /// Acceptance criteria:
    ///   • Status code is 204 No Content.
    ///   • The response body is empty.
    /// </summary>
    [Test]
    [Description("🟡 Verify DELETE /api/users/2 returns 204 No Content.")]
    public async Task DeleteUser_ExistingUser_ShouldReturn204_WithEmptyBody()
    {
        var response = await Client.DeleteAsync("/api/users/2");
        var body = await response.Content.ReadAsStringAsync();

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NoContent),
            "Expected 204 No Content");
        Assert.That(body, Is.Empty,
            "Expected an empty response body for 204");
    }

    // -------------------------------------------------------------------------
    // 🟠 Advanced tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Test: Paginated GET /api/users returns the correct page metadata.
    ///
    /// Steps:
    ///   1. Send GET /api/users?page=2.
    ///
    /// Acceptance criteria:
    ///   • Status code is 200 OK.
    ///   • The "page" field in the response equals 2.
    ///   • The "per_page" field is greater than 0.
    ///   • The "total_pages" field is greater than 0.
    ///   • The "data" array length equals the "per_page" value (or less on the last page).
    /// </summary>
    [Test]
    [Description("🟠 Verify GET /api/users?page=2 returns correct pagination metadata.")]
    public async Task GetUsers_Page2_ShouldReturnCorrectPaginationMetadata()
    {
        var response = await Client.GetAsync("/api/users?page=2");
        var body = await ApiClient.DeserialiseAsync<UserListResponse>(response);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK),
            "Expected 200 OK");
        Assert.That(body?.Page, Is.EqualTo(2),
            "Expected page field to equal 2");
        Assert.That(body?.PerPage, Is.GreaterThan(0),
            "Expected per_page to be greater than 0");
        Assert.That(body?.TotalPages, Is.GreaterThan(0),
            "Expected total_pages to be greater than 0");
        Assert.That(body?.Data.Count, Is.LessThanOrEqualTo(body?.PerPage),
            "Expected data array length to not exceed per_page");
    }

    /// <summary>
    /// Test: Each user ID in a data-driven set can be fetched individually.
    ///
    /// Steps:
    ///   1. For each user ID supplied by the test case source:
    ///      a. Send GET /api/users/{id}.
    ///      b. Assert status is 200.
    ///      c. Assert the "id" in the response matches the requested ID.
    ///
    /// Acceptance criteria:
    ///   • Each request returns 200 OK.
    ///   • The "id" field in each response matches the requested ID.
    ///
    /// Hint: The user IDs are already provided via [TestCaseSource] below —
    ///       use the <c>userId</c> parameter to build the endpoint path.
    /// </summary>
    [Test]
    [TestCaseSource(nameof(KnownUserIds))]
    [Description("🟠 Verify each known user ID can be fetched individually with correct data.")]
    public async Task GetEachUser_ByKnownId_ShouldReturnMatchingId(int userId)
    {
        var response = await Client.GetAsync($"/api/users/{userId}");
        var body = await ApiClient.DeserialiseAsync<SingleUserResponse>(response);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK),
            $"Expected 200 OK for user id {userId}");
        Assert.That(body?.Data.Id, Is.EqualTo(userId),
            $"Expected response id to match requested id {userId}");
    }

    private static IEnumerable<int> KnownUserIds() => [1, 2, 3, 4, 5, 6];
}
