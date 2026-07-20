using NUnit.Framework;
using TestAutomation.Core.Base;
using TestAutomation.PageObjects.SauceDemo;

namespace TestAutomation.Challenges.UI.SauceDemo;

/// <summary>
/// 🟢 CHALLENGE 1 — Login Functionality (Beginner)
///
/// Objective:
///   Verify that the SauceDemo login page behaves correctly for both
///   valid and invalid credential combinations.
///
/// Skills practised:
///   • Navigating to a URL using a Page Object
///   • Filling form fields and clicking buttons
///   • Asserting on page state and URL changes
///   • Writing negative test cases
///
/// Credentials reference — use these in every test, never hard-code strings:
///   Settings.SauceDemoStandardUser  →  "standard_user"
///   Settings.SauceDemoPassword      →  "secret_sauce"  (shared by all accounts)
///   Settings.SauceDemoLockedUser    →  "locked_out_user"
///
///   Example usage:
///   <code>
///   await _loginPage.LoginAsync(Settings.SauceDemoStandardUser, Settings.SauceDemoPassword);
///   </code>
///
/// Acceptance criteria are stated in each test's XML doc comment.
/// Implement ONLY the method body — do not change method signatures or attributes.
/// </summary>
[TestFixture]
[Category("SauceDemo")]
[Category("Login")]
public sealed class Challenge1_LoginTests : BaseTest
{
    private LoginPage _loginPage = null!;

    [SetUp]
    public async Task ChallengeSetUpAsync()
    {
        _loginPage = new LoginPage(Page);
        await _loginPage.OpenAsync();
    }

    // -------------------------------------------------------------------------
    // 🟢 Beginner tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Test: Valid credentials redirect to the products page.
    ///
    /// Steps:
    ///   1. Navigate to the SauceDemo login page (already done in SetUp).
    ///   2. Enter <c>Settings.SauceDemoStandardUser</c> as the username.
    ///   3. Enter <c>Settings.SauceDemoPassword</c> as the password.
    ///   4. Click the Login button.
    ///
    /// Acceptance criteria:
    ///   • The browser URL contains "inventory".
    ///   • The products page title reads "Products".
    /// </summary>
    [Test]
    [Description("🟢 Verify that a valid user can log in and is redirected to the products page.")]
    public async Task Login_WithValidCredentials_ShouldRedirectToProductsPage()
    {
        var productsPage = await _loginPage.LoginAsync(
            Settings.SauceDemoStandardUser,
            Settings.SauceDemoPassword);

        Assert.That(Page.Url, Does.Contain("inventory"),
            "Expected URL to contain 'inventory' after successful login");
        Assert.That(await productsPage.GetPageTitleAsync(), Is.EqualTo("Products"),
            "Expected products page title to read 'Products'");
    }

    /// <summary>
    /// Test: Invalid password shows an error message.
    ///
    /// Steps:
    ///   1. Navigate to the SauceDemo login page (already done in SetUp).
    ///   2. Enter <c>Settings.SauceDemoStandardUser</c> as the username.
    ///   3. Enter any string that is NOT <c>Settings.SauceDemoPassword</c> as the password.
    ///   4. Click the Login button.
    ///
    /// Acceptance criteria:
    ///   • The user remains on the login page.
    ///   • An error banner is visible.
    ///   • The error message contains the text "Username and password do not match".
    /// </summary>
    [Test]
    [Description("🟢 Verify that an incorrect password displays an error and prevents login.")]
    public async Task Login_WithInvalidPassword_ShouldDisplayErrorMessage()
    {
        await _loginPage.AttemptLoginAsync(Settings.SauceDemoStandardUser, "wrong_password");

        Assert.That(await _loginPage.IsDisplayedAsync(), Is.True,
            "Expected user to remain on the login page after invalid password");
        Assert.That(await _loginPage.IsErrorDisplayedAsync(), Is.True,
            "Expected an error banner to be visible");
        Assert.That(await _loginPage.GetErrorMessageAsync(),
            Does.Contain("Username and password do not match"),
            "Expected error message to mention mismatched credentials");
    }

    /// <summary>
    /// Test: Empty username field shows a validation error.
    ///
    /// Steps:
    ///   1. Navigate to the SauceDemo login page (already done in SetUp).
    ///   2. Leave the username field empty — do not call EnterUsernameAsync.
    ///   3. Enter <c>Settings.SauceDemoPassword</c> as the password.
    ///   4. Click the Login button.
    ///
    /// Acceptance criteria:
    ///   • An error banner is visible.
    ///   • The error message contains the text "Username is required".
    /// </summary>
    [Test]
    [Description("🟢 Verify that submitting the form with an empty username shows a validation error.")]
    public async Task Login_WithEmptyUsername_ShouldShowUsernameRequiredError()
    {
        await _loginPage.AttemptLoginAsync(string.Empty, Settings.SauceDemoPassword);

        Assert.That(await _loginPage.IsErrorDisplayedAsync(), Is.True,
            "Expected a validation error banner to be visible");
        Assert.That(await _loginPage.GetErrorMessageAsync(),
            Does.Contain("Username is required"),
            "Expected error message to say 'Username is required'");
    }

    // -------------------------------------------------------------------------
    // 🟡 Intermediate tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Test: Locked-out user cannot log in.
    ///
    /// Steps:
    ///   1. Navigate to the SauceDemo login page (already done in SetUp).
    ///   2. Enter <c>Settings.SauceDemoLockedUser</c> as the username.
    ///   3. Enter <c>Settings.SauceDemoPassword</c> as the password.
    ///   4. Click the Login button.
    ///
    /// Acceptance criteria:
    ///   • The user remains on the login page.
    ///   • The error message contains "Sorry, this user has been locked out".
    /// </summary>
    [Test]
    [Description("🟡 Verify that a locked-out user sees a specific error message and cannot log in.")]
    public async Task Login_WithLockedOutUser_ShouldDisplayLockedOutError()
    {
        await _loginPage.AttemptLoginAsync(
            Settings.SauceDemoLockedUser,
            Settings.SauceDemoPassword);

        Assert.That(await _loginPage.IsDisplayedAsync(), Is.True,
            "Expected user to remain on the login page");
        Assert.That(await _loginPage.GetErrorMessageAsync(),
            Does.Contain("Sorry, this user has been locked out"),
            "Expected locked-out error message");
    }

    /// <summary>
    /// Test: Empty password field shows a validation error.
    ///
    /// Steps:
    ///   1. Navigate to the SauceDemo login page (already done in SetUp).
    ///   2. Enter <c>Settings.SauceDemoStandardUser</c> as the username.
    ///   3. Leave the password field empty — do not call EnterPasswordAsync.
    ///   4. Click the Login button.
    ///
    /// Acceptance criteria:
    ///   • An error banner is visible.
    ///   • The error message contains "Password is required".
    /// </summary>
    [Test]
    [Description("🟡 Verify that submitting the form with an empty password shows a validation error.")]
    public async Task Login_WithEmptyPassword_ShouldShowPasswordRequiredError()
    {
        await _loginPage.AttemptLoginAsync(Settings.SauceDemoStandardUser, string.Empty);

        Assert.That(await _loginPage.IsErrorDisplayedAsync(), Is.True,
            "Expected a validation error banner to be visible");
        Assert.That(await _loginPage.GetErrorMessageAsync(),
            Does.Contain("Password is required"),
            "Expected error message to say 'Password is required'");
    }

    // -------------------------------------------------------------------------
    // 🟠 Advanced tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Test: All five standard SauceDemo usernames can log in successfully.
    ///
    /// Steps:
    ///   1. For each username supplied by <see cref="ValidUsernames"/>:
    ///      a. Navigate to the login page.
    ///      b. Enter the username and <c>Settings.SauceDemoPassword</c> as the password.
    ///      c. Click the Login button.
    ///
    /// Acceptance criteria:
    ///   • Each user is redirected to the inventory page after login.
    ///
    /// Hint: The usernames are already provided via [TestCaseSource] below —
    ///       use <c>Settings.SauceDemoPassword</c> as the password for all of them.
    /// </summary>
    [Test]
    [TestCaseSource(nameof(ValidUsernames))]
    [Description("🟠 Verify that each valid SauceDemo user account can successfully authenticate.")]
    public async Task Login_WithEachValidUser_ShouldSucceed(string username)
    {
        await _loginPage.LoginAsync(username, Settings.SauceDemoPassword);

        Assert.That(Page.Url, Does.Contain("inventory"),
            $"Expected '{username}' to be redirected to inventory after login");
    }

    private static IEnumerable<string> ValidUsernames() =>
    [
        "standard_user",
        "problem_user",
        "performance_glitch_user",
        "error_user",
        "visual_user"
    ];
}