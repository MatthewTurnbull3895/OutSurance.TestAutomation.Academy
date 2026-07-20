using Microsoft.Extensions.Configuration;

namespace TestAutomation.Core.Config;

/// <summary>
/// Builds a <see cref="TestSettings"/> instance by layering configuration sources
/// in the following priority order (highest wins):
///
///   1. Environment variables       — CI / local overrides
///   2. testsettings.json           — project-level defaults, committed to source
///
/// The JSON file is resolved by walking up from the test output directory
/// until the file is found, so it works regardless of the working directory.
///
/// Secrets (e.g. <c>ReqResApiKey</c>) must never be placed in
/// <c>testsettings.json</c>. Contact the test automation team to
/// obtain the key and set it as an environment variable instead.
/// </summary>
public static class SettingsLoader
{
    private const string SettingsFileName = "testsettings.json";

    /// <summary>
    /// Builds and returns a fresh <see cref="TestSettings"/> instance on every call,
    /// ensuring environment variables are always read at the time of the call.
    /// <see cref="TestSettings.Instance"/> caches the result via its own
    /// <see cref="Lazy{T}"/> so tests pay the build cost only once per process.
    /// </summary>
    public static TestSettings Load()
    {
        var jsonPath = ResolveJsonPath();

        // Build an explicit env-var mapping so well-known variable names
        // (e.g. REQRES_API_KEY) bind to the correct POCO property names
        // (e.g. ReqResApiKey) regardless of casing or underscore conventions.
        var envOverrides = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            ["Browser"]              = Env("Browser")              ?? Env("TEST_BROWSER"),
            ["Headless"]             = Env("Headless")             ?? Env("TEST_HEADLESS"),
            ["SlowMo"]               = Env("SlowMo")               ?? Env("TEST_SLOW_MO"),
            ["DefaultTimeoutMs"]     = Env("DefaultTimeoutMs")     ?? Env("TEST_TIMEOUT"),
            ["SauceDemoBaseUrl"]     = Env("SauceDemoBaseUrl")     ?? Env("SAUCEDEMO_BASE_URL"),
            ["TheInternetBaseUrl"]   = Env("TheInternetBaseUrl")   ?? Env("THE_INTERNET_BASE_URL"),
            ["DemoQaBaseUrl"]        = Env("DemoQaBaseUrl")        ?? Env("DEMOQA_BASE_URL"),
            ["ReqResBaseUrl"]        = Env("ReqResBaseUrl")        ?? Env("REQRES_BASE_URL"),
            ["SauceDemoStandardUser"]= Env("SauceDemoStandardUser")?? Env("SAUCEDEMO_USER"),
            ["SauceDemoPassword"]    = Env("SauceDemoPassword")    ?? Env("SAUCEDEMO_PASSWORD"),
            ["ReqResApiKey"]         = Env("ReqResApiKey")         ?? Env("REQRES_API_KEY"),
        };

        // Remove nulls so they don't overwrite JSON / defaults
        var filteredOverrides = envOverrides
            .Where(kv => kv.Value is not null)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        var config = new ConfigurationBuilder()
            .AddJsonFile(jsonPath, optional: true, reloadOnChange: false)
            .AddInMemoryCollection(filteredOverrides)
            .Build();

        return config.Get<TestSettings>() ?? new TestSettings();
    }

    private static string? Env(string name) =>
        Environment.GetEnvironmentVariable(name);

    // -------------------------------------------------------------------------
    // Private
    // -------------------------------------------------------------------------

    /// <summary>
    /// Walks up the directory tree from the current working directory until
    /// <c>testsettings.json</c> is found, or returns the CWD path as a
    /// fallback (treated as optional / missing by the builder).
    /// </summary>
    private static string ResolveJsonPath()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, SettingsFileName);
            if (File.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        return Path.Combine(Directory.GetCurrentDirectory(), SettingsFileName);
    }
}
