using NUnit.Framework;
using TestAutomation.Core.Base;
using TestAutomation.PageObjects.SauceDemo;

namespace TestAutomation.Challenges.UI.SauceDemo;

/// <summary>
/// 🟡 CHALLENGE 2 — Shopping Cart Functionality (Intermediate → Advanced)
///
/// Objective:
///   Verify that products can be added to and removed from the SauceDemo
///   shopping cart, and that the cart accurately reflects those changes.
///
/// Skills practised:
///   • Multi-step user journeys across several pages
///   • Asserting on dynamic UI elements (cart badge count, item lists)
///   • State verification before and after actions
///   • Data-driven tests using [TestCaseSource]
///
/// Setup:
///   Each test begins already logged in as the standard user.
///   The <see cref="LoginAsStandardUserAsync"/> helper (at the bottom of this file)
///   handles this automatically in [SetUp] — you do not need to log in yourself.
///
///   Credentials used by the helper (for reference only):
///   Settings.SauceDemoStandardUser  →  "standard_user"
///   Settings.SauceDemoPassword      →  "secret_sauce"
///
/// Acceptance criteria are stated in each test's XML doc comment.
/// Implement ONLY the method body — do not change method signatures or attributes.
/// </summary>
[TestFixture]
[Category("SauceDemo")]
[Category("Cart")]
public sealed class Challenge2_CartTests : BaseTest
{
    private ProductsPage _productsPage = null!;

    [SetUp]
    public async Task ChallengeSetUpAsync() =>
        _productsPage = await LoginAsStandardUserAsync();

    // -------------------------------------------------------------------------
    // 🟢 Beginner tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Test: Adding one product updates the cart badge to 1.
    ///
    /// Steps:
    ///   1. On the products page, add "Sauce Labs Backpack" to the cart.
    ///
    /// Acceptance criteria:
    ///   • The cart icon badge displays the number 1.
    /// </summary>
    [Test]
    [Description("🟢 Verify the cart badge increments to 1 when a single product is added.")]
    public async Task AddProduct_SingleItem_ShouldShowBadgeCountOfOne()
    {
        await _productsPage.AddProductToCartAsync("Sauce Labs Backpack");

        Assert.That(await _productsPage.GetCartBadgeCountAsync(), Is.EqualTo(1),
            "Expected cart badge to show 1 after adding one item");
    }

    /// <summary>
    /// Test: The cart page shows the correct product after it is added.
    ///
    /// Steps:
    ///   1. Add "Sauce Labs Bike Light" to the cart.
    ///   2. Navigate to the cart page.
    ///
    /// Acceptance criteria:
    ///   • The cart contains exactly 1 item.
    ///   • The item name is "Sauce Labs Bike Light".
    /// </summary>
    [Test]
    [Description("🟢 Verify the correct product appears in the cart after being added from the inventory.")]
    public async Task AddProduct_NavigateToCart_ShouldDisplayCorrectItem()
    {
        await _productsPage.AddProductToCartAsync("Sauce Labs Bike Light");
        var cartPage = await _productsPage.GoToCartAsync();

        Assert.That(await cartPage.GetCartItemCountAsync(), Is.EqualTo(1),
            "Expected exactly 1 item in the cart");
        Assert.That(await cartPage.ContainsProductAsync("Sauce Labs Bike Light"), Is.True,
            "Expected 'Sauce Labs Bike Light' to be present in the cart");
    }

    // -------------------------------------------------------------------------
    // 🟡 Intermediate tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Test: Adding multiple products updates the badge count correctly.
    ///
    /// Steps:
    ///   1. Add "Sauce Labs Backpack" to the cart.
    ///   2. Add "Sauce Labs Bike Light" to the cart.
    ///   3. Add "Sauce Labs Bolt T-Shirt" to the cart.
    ///
    /// Acceptance criteria:
    ///   • The cart badge displays the number 3.
    ///   • The cart page contains all three product names.
    /// </summary>
    [Test]
    [Description("🟡 Verify the cart badge and cart contents are correct after adding multiple items.")]
    public async Task AddMultipleProducts_ShouldReflectCorrectBadgeAndCartContents()
    {
        await _productsPage.AddProductToCartAsync("Sauce Labs Backpack");
        await _productsPage.AddProductToCartAsync("Sauce Labs Bike Light");
        await _productsPage.AddProductToCartAsync("Sauce Labs Bolt T-Shirt");

        Assert.That(await _productsPage.GetCartBadgeCountAsync(), Is.EqualTo(3),
            "Expected cart badge to show 3 after adding three items");

        var cartPage = await _productsPage.GoToCartAsync();
        var names = await cartPage.GetCartItemNamesAsync();

        Assert.That(names, Does.Contain("Sauce Labs Backpack"), "Expected Backpack in cart");
        Assert.That(names, Does.Contain("Sauce Labs Bike Light"), "Expected Bike Light in cart");
        Assert.That(names, Does.Contain("Sauce Labs Bolt T-Shirt"), "Expected Bolt T-Shirt in cart");
    }

    /// <summary>
    /// Test: Removing a product from the cart updates the cart correctly.
    ///
    /// Steps:
    ///   1. Add "Sauce Labs Backpack" and "Sauce Labs Bike Light" to the cart.
    ///   2. Navigate to the cart page.
    ///   3. Remove "Sauce Labs Backpack".
    ///
    /// Acceptance criteria:
    ///   • The cart contains exactly 1 item.
    ///   • The remaining item is "Sauce Labs Bike Light".
    ///   • "Sauce Labs Backpack" is no longer in the cart.
    /// </summary>
    [Test]
    [Description("🟡 Verify that removing one item from a multi-item cart leaves the remaining item intact.")]
    public async Task RemoveProduct_FromCartWithMultipleItems_ShouldUpdateCartCorrectly()
    {
        await _productsPage.AddProductToCartAsync("Sauce Labs Backpack");
        await _productsPage.AddProductToCartAsync("Sauce Labs Bike Light");
        var cartPage = await _productsPage.GoToCartAsync();

        await cartPage.RemoveProductAsync("Sauce Labs Backpack");

        Assert.That(await cartPage.GetCartItemCountAsync(), Is.EqualTo(1),
            "Expected exactly 1 item remaining after removal");
        Assert.That(await cartPage.ContainsProductAsync("Sauce Labs Bike Light"), Is.True,
            "Expected 'Sauce Labs Bike Light' to still be in the cart");
        Assert.That(await cartPage.ContainsProductAsync("Sauce Labs Backpack"), Is.False,
            "Expected 'Sauce Labs Backpack' to have been removed");
    }

    /// <summary>
    /// Test: Continuing shopping from the cart returns the user to the products page.
    ///
    /// Steps:
    ///   1. Add any product to the cart.
    ///   2. Navigate to the cart page.
    ///   3. Click "Continue Shopping".
    ///
    /// Acceptance criteria:
    ///   • The browser URL contains "inventory".
    ///   • The products page title is visible.
    /// </summary>
    [Test]
    [Description("🟡 Verify that Continue Shopping navigates back to the inventory page.")]
    public async Task ContinueShopping_FromCart_ShouldReturnToProductsPage()
    {
        await _productsPage.AddProductToCartAsync("Sauce Labs Backpack");
        var cartPage = await _productsPage.GoToCartAsync();
        var productsPage = await cartPage.ContinueShoppingAsync();

        Assert.That(Page.Url, Does.Contain("inventory"),
            "Expected URL to contain 'inventory' after clicking Continue Shopping");
        Assert.That(await productsPage.IsDisplayedAsync(), Is.True,
            "Expected the products page title to be visible");
    }

    // -------------------------------------------------------------------------
    // 🟠 Advanced tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Test: Cart persists its contents after navigating away and returning.
    ///
    /// Steps:
    ///   1. Add "Sauce Labs Fleece Jacket" to the cart.
    ///   2. Navigate to the cart page.
    ///   3. Click "Continue Shopping" to return to the products page.
    ///   4. Navigate to the cart page again.
    ///
    /// Acceptance criteria:
    ///   • The cart still contains "Sauce Labs Fleece Jacket".
    ///   • The cart badge still shows 1.
    /// </summary>
    [Test]
    [Description("🟠 Verify that cart contents persist when navigating away and returning to the cart.")]
    public async Task Cart_ShouldPersistContents_AfterNavigatingAwayAndBack()
    {
        await _productsPage.AddProductToCartAsync("Sauce Labs Fleece Jacket");
        var cartPage = await _productsPage.GoToCartAsync();
        var productsPage = await cartPage.ContinueShoppingAsync();
        cartPage = await productsPage.GoToCartAsync();

        Assert.That(await cartPage.ContainsProductAsync("Sauce Labs Fleece Jacket"), Is.True,
            "Expected cart to still contain 'Sauce Labs Fleece Jacket' after navigating away and back");
        Assert.That(await productsPage.GetCartBadgeCountAsync(), Is.EqualTo(1),
            "Expected cart badge to still show 1");
    }

    /// <summary>
    /// Test: Each product in a data-driven set can be individually added and verified.
    ///
    /// Steps:
    ///   1. For each product name supplied by the test case source:
    ///      a. Add the product to the cart.
    ///      b. Navigate to the cart.
    ///      c. Verify the product is present.
    ///      d. Remove the product to reset state for the next iteration.
    ///
    /// Acceptance criteria:
    ///   • Each product name appears in the cart when added.
    ///   • The cart is empty after the product is removed.
    ///
    /// Hint: The product names are already provided via [TestCaseSource] below —
    ///       use <c>_productsPage</c> to add and <c>CartPage</c> to remove.
    /// </summary>
    [Test]
    [TestCaseSource(nameof(SauceDemoProducts))]
    [Description("🟠 Verify each product can be independently added to and removed from the cart.")]
    public async Task AddAndRemoveEachProduct_ShouldMaintainCartAccuracy(string productName)
    {
        await _productsPage.AddProductToCartAsync(productName);
        var cartPage = await _productsPage.GoToCartAsync();

        Assert.That(await cartPage.ContainsProductAsync(productName), Is.True,
            $"Expected '{productName}' to appear in the cart after being added");

        await cartPage.RemoveProductAsync(productName);

        Assert.That(await cartPage.IsEmptyAsync(), Is.True,
            $"Expected cart to be empty after removing '{productName}'");
    }

    private static IEnumerable<string> SauceDemoProducts() =>
    [
        "Sauce Labs Backpack",
        "Sauce Labs Bike Light",
        "Sauce Labs Bolt T-Shirt",
        "Sauce Labs Fleece Jacket",
        "Sauce Labs Onesie",
        "Test.allTheThings() T-Shirt (Red)"
    ];

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Logs in as the standard SauceDemo user and returns the products page.
    /// </summary>
    private async Task<ProductsPage> LoginAsStandardUserAsync()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.OpenAsync();
        return await loginPage.LoginAsync(
            Settings.SauceDemoStandardUser,
            Settings.SauceDemoPassword);
    }
}