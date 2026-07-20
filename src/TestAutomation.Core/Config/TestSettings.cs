using Microsoft.Playwright;

namespace TestAutomation.Core.Config;

/// <summary>
/// Strongly-typed settings POCO for the test automation framework.
/// Property values are populated by <see cref="SettingsLoader"/> from
/// a <c>testsettings.json</c> file in the test project, with environment
/// variables taking precedence over JSON values.
///
/// No environment-variable reads or defaults live in this class —
/// all of that logic is centralised in <see cref="SettingsLoader"/>.
/// </summary>
public sealed class TestSettings
{
    // -------------------------------------------------------------------------
    // Browser
    // -------------------------------------------------------------------------

    /// <summary>Browser to launch: chromium | firefox | webkit. Default: chromium.</summary>
    public string Browser { get; set; } = "chromium";

    /// <summary>Run the browser without a visible window. Default: true.</summary>
    public bool Headless { get; set; } = true;

    /// <summary>Milliseconds added between every Playwright action. Default: 0.</summary>
    public float SlowMo { get; set; } = 0f;

    /// <summary>Default action / navigation timeout in milliseconds. Default: 30000.</summary>
    public float DefaultTimeoutMs { get; set; } = 30_000f;

    // -------------------------------------------------------------------------
    // Base URLs
    // -------------------------------------------------------------------------

    /// <summary>Base URL for https://www.saucedemo.com</summary>
    public string SauceDemoBaseUrl { get; set; } = "https://www.saucedemo.com";

    /// <summary>Base URL for https://the-internet.herokuapp.com</summary>
    public string TheInternetBaseUrl { get; set; } = "https://the-internet.herokuapp.com";

    /// <summary>Base URL for https://demoqa.com</summary>
    public string DemoQaBaseUrl { get; set; } = "https://demoqa.com";

    /// <summary>Base URL for https://reqres.in</summary>
    public string ReqResBaseUrl { get; set; } = "https://reqres.in";

    // -------------------------------------------------------------------------
    // SauceDemo credentials
    // -------------------------------------------------------------------------

    /// <summary>Standard valid username for SauceDemo.</summary>
    public string SauceDemoStandardUser { get; set; } = "standard_user";

    /// <summary>Shared password for all SauceDemo test accounts.</summary>
    public string SauceDemoPassword { get; set; } = "secret_sauce";

    /// <summary>Locked-out username for negative login tests.</summary>
    public string SauceDemoLockedUser { get; set; } = "locked_out_user";

    /// <summary>Problem user for visual/regression tests.</summary>
    public string SauceDemoProblemUser { get; set; } = "problem_user";

    // -------------------------------------------------------------------------
    // API keys
    // -------------------------------------------------------------------------

    /// <summary>
    /// API key for https://reqres.in.
    /// Do not add this value here or in testsettings.json.
    /// Contact the test automation team to obtain the key and set it
    /// as the REQRES_API_KEY environment variable on your machine.
    /// </summary>
    public string ReqResApiKey { get; set; } = string.Empty;

    // -------------------------------------------------------------------------
    // Singleton — populated once by SettingsLoader
    // -------------------------------------------------------------------------

    private static readonly Lazy<TestSettings> _instance =
        new(SettingsLoader.Load, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// The single shared instance, built once per process from
    /// <c>testsettings.json</c> and environment variables by <see cref="SettingsLoader"/>.
    /// </summary>
    public static TestSettings Instance => _instance.Value;

    // -------------------------------------------------------------------------
    // Playwright browser-type helper
    // -------------------------------------------------------------------------

    /// <summary>
    /// Resolves the <see cref="Browser"/> string to the matching
    /// Playwright <see cref="IBrowserType"/>.
    /// </summary>
    public IBrowserType ResolveBrowserType(IPlaywright playwright) =>
        Browser switch
        {
            "firefox" => playwright.Firefox,
            "webkit"  => playwright.Webkit,
            _         => playwright.Chromium
        };
}
