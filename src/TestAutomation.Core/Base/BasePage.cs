using Microsoft.Playwright;
using TestAutomation.Core.Config;

namespace TestAutomation.Core.Base;

/// <summary>
/// Abstract base class for all Page Objects.
/// Wraps the Playwright <see cref="IPage"/> and exposes
/// reusable, framework-level helper methods.
/// Page Objects that inherit this class must never expose raw locators;
/// all interactions are surfaced through named methods.
/// </summary>
public abstract class BasePage
{
    /// <summary>The underlying Playwright page instance.</summary>
    protected readonly IPage Page;

    /// <summary>Framework-wide settings.</summary>
    protected readonly TestSettings Settings;

    /// <summary>
    /// Initialises a new <see cref="BasePage"/>.
    /// </summary>
    /// <param name="page">The Playwright <see cref="IPage"/> to wrap.</param>
    protected BasePage(IPage page)
    {
        Page     = page ?? throw new ArgumentNullException(nameof(page));
        Settings = TestSettings.Instance;
    }

    // -------------------------------------------------------------------------
    // Navigation
    // -------------------------------------------------------------------------

    /// <summary>
    /// Navigates to the given <paramref name="url"/> and waits until the
    /// network is idle (DOMContentLoaded by default).
    /// </summary>
    public virtual async Task NavigateToAsync(string url) =>
        await Page.GotoAsync(url, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        });

    /// <summary>Returns the current page URL.</summary>
    public string CurrentUrl => Page.Url;

    /// <summary>Returns the current page title.</summary>
    public async Task<string> GetTitleAsync() => await Page.TitleAsync();

    // -------------------------------------------------------------------------
    // Element interaction helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Clicks the element identified by <paramref name="locator"/>.
    /// Playwright's auto-waiting ensures the element is actionable first.
    /// </summary>
    protected async Task ClickAsync(ILocator locator) =>
        await locator.ClickAsync();

    /// <summary>
    /// Types <paramref name="text"/> into the element identified by
    /// <paramref name="locator"/>, clearing any existing value first.
    /// </summary>
    protected async Task FillAsync(ILocator locator, string text) =>
        await locator.FillAsync(text);

    /// <summary>
    /// Returns the trimmed inner text of the element.
    /// </summary>
    protected async Task<string> GetTextAsync(ILocator locator) =>
        (await locator.InnerTextAsync()).Trim();

    /// <summary>
    /// Returns <c>true</c> when the element is visible on the page.
    /// </summary>
    protected async Task<bool> IsVisibleAsync(ILocator locator) =>
        await locator.IsVisibleAsync();

    /// <summary>
    /// Selects an option from a <c>&lt;select&gt;</c> element by its visible label.
    /// </summary>
    protected async Task SelectByLabelAsync(ILocator locator, string label) =>
        await locator.SelectOptionAsync(new SelectOptionValue { Label = label });

    /// <summary>
    /// Selects an option from a <c>&lt;select&gt;</c> element by its value attribute.
    /// </summary>
    protected async Task SelectByValueAsync(ILocator locator, string value) =>
        await locator.SelectOptionAsync(new SelectOptionValue { Value = value });

    // -------------------------------------------------------------------------
    // Waiting helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Waits for the element to be visible. Relies on Playwright auto-waiting;
    /// provided as an explicit hook for edge cases where visibility must be
    /// confirmed before continuing the flow.
    /// </summary>
    protected async Task WaitForVisibleAsync(ILocator locator) =>
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible
        });

    /// <summary>
    /// Waits for the element to be hidden / detached.
    /// </summary>
    protected async Task WaitForHiddenAsync(ILocator locator) =>
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Hidden
        });

    /// <summary>
    /// Waits until the page URL contains <paramref name="urlFragment"/>.
    /// </summary>
    protected async Task WaitForUrlContainsAsync(string urlFragment) =>
        await Page.WaitForURLAsync(url => url.Contains(urlFragment, StringComparison.OrdinalIgnoreCase));

    // -------------------------------------------------------------------------
    // Screenshot / diagnostics
    // -------------------------------------------------------------------------

    /// <summary>
    /// Captures a full-page screenshot to the given <paramref name="path"/>.
    /// Called automatically by <see cref="BaseTest"/> on test failure.
    /// </summary>
    public async Task CaptureScreenshotAsync(string path) =>
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path     = path,
            FullPage = true
        });
}