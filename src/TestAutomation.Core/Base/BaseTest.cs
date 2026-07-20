using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TestAutomation.Core.Config;

namespace TestAutomation.Core.Base;

/// <summary>
/// Abstract NUnit base class for all UI test fixtures.
/// Manages the full Playwright IPlaywright / browser / context / page lifecycle
/// and wires up automatic screenshot capture on test failure.
///
/// Inherit this class in every UI test fixture:
/// <code>
/// public class MyTests : BaseTest { ... }
/// </code>
/// </summary>
[TestFixture]
public abstract class BaseTest
{
    // -------------------------------------------------------------------------
    // Protected Playwright objects — available to all inheriting test fixtures
    // -------------------------------------------------------------------------

    /// <summary>The root Playwright instance shared across the fixture.</summary>
    protected IPlaywright PlaywrightInstance { get; private set; } = null!;

    /// <summary>The browser instance shared across the fixture.</summary>
    protected IBrowser Browser { get; private set; } = null!;

    /// <summary>
    /// An isolated browser context created fresh for each test,
    /// guaranteeing no cookie / storage bleed between tests.
    /// </summary>
    protected IBrowserContext Context { get; private set; } = null!;

    /// <summary>A new page opened inside <see cref="Context"/>.</summary>
    protected IPage Page { get; private set; } = null!;

    /// <summary>Framework settings resolved from environment variables.</summary>
    protected TestSettings Settings => TestSettings.Instance;

    // -------------------------------------------------------------------------
    // Screenshot output directory — resolved lazily to avoid static init issues
    // -------------------------------------------------------------------------

    private string ScreenshotDirectory =>
        Path.Combine(TestContext.CurrentContext.WorkDirectory, "screenshots");

    // -------------------------------------------------------------------------
    // One-time setup — create IPlaywright and launch browser once per fixture
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates the Playwright instance and launches the configured browser
    /// once for the entire test fixture.
    /// Browser type and headless mode are controlled via <see cref="TestSettings"/>.
    /// </summary>
    [OneTimeSetUp]
    public async Task OneTimeSetUpAsync()
    {
        PlaywrightInstance = await Microsoft.Playwright.Playwright.CreateAsync();

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = Settings.Headless,
            SlowMo   = Settings.SlowMo
        };

        Browser = Settings.Browser switch
        {
            "firefox" => await PlaywrightInstance.Firefox.LaunchAsync(launchOptions),
            "webkit"  => await PlaywrightInstance.Webkit.LaunchAsync(launchOptions),
            _         => await PlaywrightInstance.Chromium.LaunchAsync(launchOptions)
        };

        Directory.CreateDirectory(ScreenshotDirectory);
    }

    // -------------------------------------------------------------------------
    // Per-test setup — fresh context and page for every test
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates a new <see cref="IBrowserContext"/> and <see cref="IPage"/>
    /// before each test, ensuring full isolation.
    /// </summary>
    [SetUp]
    public async Task SetUpAsync()
    {
        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            BaseURL      = Settings.SauceDemoBaseUrl
        });

        Context.SetDefaultTimeout(Settings.DefaultTimeoutMs);

        Page = await Context.NewPageAsync();
    }

    // -------------------------------------------------------------------------
    // Per-test teardown — screenshot on failure, then dispose
    // -------------------------------------------------------------------------

    /// <summary>
    /// After each test:
    /// <list type="bullet">
    ///   <item>Captures a full-page screenshot when the test failed.</item>
    ///   <item>Closes and disposes the browser context.</item>
    /// </list>
    /// </summary>
    [TearDown]
    public async Task TearDownAsync()
    {
        try
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                var testName  = TestContext.CurrentContext.Test.FullName
                    .Replace(' ', '_')
                    .Replace('(', '_')
                    .Replace(')', '_');

                var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                var path      = Path.Combine(ScreenshotDirectory, $"{testName}_{timestamp}.png");

                await Page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path     = path,
                    FullPage = true
                });

                TestContext.AddTestAttachment(path, "Failure screenshot");
                TestContext.WriteLine($"Screenshot saved: {path}");
            }
        }
        finally
        {
            await Context.DisposeAsync();
        }
    }

    // -------------------------------------------------------------------------
    // One-time teardown — close the browser
    // -------------------------------------------------------------------------

    /// <summary>Closes the browser and disposes the Playwright instance after all tests in the fixture have run.</summary>
    [OneTimeTearDown]
    public async Task OneTimeTearDownAsync()
    {
        await Browser.CloseAsync();
        PlaywrightInstance.Dispose();
    }
}