using NUnit.Framework;
using TestAutomation.Core.Base;
using TestAutomation.PageObjects.SauceDemo;

namespace TestAutomation.Challenges.UI;

/// <summary>
/// Framework smoke tests for the UI project.
///
/// These tests are NOT challenges — they are fully implemented and must
/// always remain green. Their sole purpose is to prove that the framework
/// plumbing is correctly wired before any candidate code is written:
///
///   • Playwright launches the configured browser without error.
///   • A new browser context and page are created per test.
///   • A Page Object can navigate to a real URL.
///   • The page title and URL can be read and asserted on.
///   • The screenshot-on-failure mechanism in BaseTest is in place.
///
/// If any of these tests fail, fix the framework before attempting challenges.
/// </summary>
[TestFixture]
[Category("Smoke")]
public sealed class FrameworkSmokeTests : BaseTest
{
    /// <summary>
    /// Verifies that Playwright can launch a browser, open a page, and
    /// navigate to the SauceDemo login page.
    /// A passing result confirms the full UI framework stack is operational.
    /// </summary>
    [Test]
    public async Task UI_Framework_BrowserLaunchesAndNavigatesToLoginPage()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.OpenAsync();

        Assert.That(Page.Url, Does.Contain("saucedemo.com"),
            "Expected the browser to have navigated to saucedemo.com");

        Assert.That(await loginPage.IsDisplayedAsync(), Is.True,
            "Expected the Login button to be visible on the SauceDemo login page");
    }

    /// <summary>
    /// Verifies that the page title can be retrieved via BasePage and
    /// matches the known SauceDemo title.
    /// </summary>
    [Test]
    public async Task UI_Framework_PageTitleCanBeRead()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.OpenAsync();

        var title = await loginPage.GetTitleAsync();

        Assert.That(title, Is.EqualTo("Swag Labs"),
            "Expected the SauceDemo page title to be 'Swag Labs'");
    }

    /// <summary>
    /// Verifies that TestSettings resolves the SauceDemo base URL
    /// and that the value is not null or empty.
    /// </summary>
    [Test]
    public void UI_Framework_TestSettings_ResolvesBaseUrls()
    {
        Assert.That(Settings.SauceDemoBaseUrl, Is.Not.Null.And.Not.Empty,
            "SauceDemoBaseUrl must be set");
        Assert.That(Settings.TheInternetBaseUrl, Is.Not.Null.And.Not.Empty,
            "TheInternetBaseUrl must be set");
        Assert.That(Settings.DemoQaBaseUrl, Is.Not.Null.And.Not.Empty,
            "DemoQaBaseUrl must be set");
    }
}
