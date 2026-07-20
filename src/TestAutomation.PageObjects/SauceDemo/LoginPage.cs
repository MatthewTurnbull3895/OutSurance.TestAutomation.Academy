using Microsoft.Playwright;
using TestAutomation.Core.Base;

namespace TestAutomation.PageObjects.SauceDemo;

/// <summary>
/// Page Object for the SauceDemo login page (https://www.saucedemo.com).
/// Encapsulates all interactions with the login form.
/// Raw locators are never exposed outside this class.
/// </summary>
public sealed class LoginPage : BasePage
{
    // -------------------------------------------------------------------------
    // Locators — private; never exposed to tests
    // -------------------------------------------------------------------------

    private ILocator UsernameInput    => Page.GetByPlaceholder("Username");
    private ILocator PasswordInput    => Page.GetByPlaceholder("Password");
    private ILocator LoginButton      => Page.GetByRole(AriaRole.Button, new() { Name = "Login" });
    private ILocator ErrorMessage     => Page.Locator("[data-test='error']");

    // -------------------------------------------------------------------------
    // Constructor
    // -------------------------------------------------------------------------

    public LoginPage(IPage page) : base(page) { }

    // -------------------------------------------------------------------------
    // Public page actions
    // -------------------------------------------------------------------------

    /// <summary>
    /// Navigates directly to the SauceDemo login page.
    /// </summary>
    public async Task OpenAsync() =>
        await NavigateToAsync(Settings.SauceDemoBaseUrl);

    /// <summary>
    /// Enters <paramref name="username"/> into the username field.
    /// </summary>
    public async Task EnterUsernameAsync(string username) =>
        await FillAsync(UsernameInput, username);

    /// <summary>
    /// Enters <paramref name="password"/> into the password field.
    /// </summary>
    public async Task EnterPasswordAsync(string password) =>
        await FillAsync(PasswordInput, password);

    /// <summary>
    /// Clicks the Login button.
    /// </summary>
    public async Task ClickLoginAsync() =>
        await ClickAsync(LoginButton);

    /// <summary>
    /// Performs a complete login in one call.
    /// Returns a <see cref="ProductsPage"/> ready for further assertions.
    /// </summary>
    /// <param name="username">SauceDemo username.</param>
    /// <param name="password">SauceDemo password.</param>
    public async Task<ProductsPage> LoginAsync(string username, string password)
    {
        await EnterUsernameAsync(username);
        await EnterPasswordAsync(password);
        await ClickLoginAsync();
        await WaitForUrlContainsAsync("inventory");
        return new ProductsPage(Page);
    }

    /// <summary>
    /// Attempts login without waiting for a successful redirect.
    /// Use this for negative tests where an error message is expected.
    /// </summary>
    public async Task AttemptLoginAsync(string username, string password)
    {
        await EnterUsernameAsync(username);
        await EnterPasswordAsync(password);
        await ClickLoginAsync();
    }

    // -------------------------------------------------------------------------
    // Public page queries
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns <c>true</c> when an error banner is visible on the login page.
    /// </summary>
    public async Task<bool> IsErrorDisplayedAsync() =>
        await IsVisibleAsync(ErrorMessage);

    /// <summary>
    /// Returns the trimmed text content of the error banner.
    /// </summary>
    public async Task<string> GetErrorMessageAsync() =>
        await GetTextAsync(ErrorMessage);

    /// <summary>
    /// Returns <c>true</c> when the login page itself is visible
    /// (i.e. the user has NOT been redirected after a login attempt).
    /// </summary>
    public async Task<bool> IsDisplayedAsync() =>
        await IsVisibleAsync(LoginButton);
}