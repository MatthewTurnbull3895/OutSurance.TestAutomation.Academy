using NUnit.Framework;
using TestAutomation.Core.Config;
using TestAutomation.Core.Http;

namespace TestAutomation.Challenges.API.Base;

/// <summary>
/// Abstract NUnit base class for all API test fixtures.
/// Creates a fresh <see cref="ApiClient"/> backed by an <see cref="HttpClient"/>
/// for each test, pointed at the configured base URL.
///
/// Inherit this class in every API test fixture:
/// <code>
/// public class MyApiTests : BaseApiTest { ... }
/// </code>
/// </summary>
[TestFixture]
public abstract class BaseApiTest
{
    /// <summary>Framework settings resolved from environment variables.</summary>
    protected TestSettings Settings => TestSettings.Instance;

    /// <summary>
    /// A pre-configured <see cref="ApiClient"/> pointing at <see cref="TestSettings.ReqResBaseUrl"/>.
    /// A new instance is created for every test to ensure isolation.
    /// </summary>
    protected ApiClient Client { get; private set; } = null!;

    private HttpClient _httpClient = null!;

    /// <summary>Creates a new <see cref="HttpClient"/> and <see cref="ApiClient"/> before each test.</summary>
    [SetUp]
    public void SetUp()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(Settings.ReqResBaseUrl)
        };
        _httpClient.DefaultRequestHeaders.Add("x-api-key", Settings.ReqResApiKey);
        Client = new ApiClient(_httpClient);
    }

    /// <summary>Disposes the <see cref="HttpClient"/> after each test.</summary>
    [TearDown]
    public void TearDown() => _httpClient.Dispose();
}
