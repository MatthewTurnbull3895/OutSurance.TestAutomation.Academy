using NUnit.Framework;
using TestAutomation.Core.Base;
using TestAutomation.PageObjects.SauceDemo;

namespace TestAutomation.Challenges.UI.SauceDemo;

/// <summary>
/// 🟡 CHALLENGE 2 — Shopping Cart Functionality (Intermediate → Advanced)
///<para>
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
///</para>
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
    ///<para>
    /// Steps:
    ///   1. On the products page, add "Sauce Labs Backpack" to the cart.
    ///</para>
    /// Acceptance criteria:
    ///   • The cart icon badge displays the number 1.
    /// </summary>
    [Test]
    [Description("🟢 Verify the cart badge increments to 1 when a single product is added.")]
    public async Task AddProduct_SingleItem_ShouldShowBadgeCountOfOne()
    {
        var productsPage = new ProductsPage(Page);

        //will find the backpackbutton and click on it
        await productsPage.AddProductToCartAsync("Sauce Labs Backpack");

        //confirm that the badge has aumber1
        var badgeCount = await productsPage.GetCartBadgeCountAsync();
        Assert.That(badgeCount, Is.EqualTo(1));
    }

    /// <summary>
    /// Test: The cart page shows the correct product after it is added.
    ///<para>
    /// Steps:
    ///   1. Add "Sauce Labs Bike Light" to the cart.
    ///   2. Navigate to the cart page.
    ///</para>
    /// Acceptance criteria:
    ///   • The cart contains exactly 1 item.
    ///   • The item name is "Sauce Labs Bike Light".
    /// </summary>
    [Test]
    [Description("🟢 Verify the correct product appears in the cart after being added from the inventory.")]
    public async Task AddProduct_NavigateToCart_ShouldDisplayCorrectItem()
    {
        var productsPage = new ProductsPage(Page);
        await productsPage.AddProductToCartAsync("Sauce Labs Bike Light");

        // Navigate to the cart and capture the returned CartPage instance
        var cartPage = await productsPage.GoToCartAsync();

        // 2. Count how many items are in the cart
        var itemCount = await cartPage.GetCartItemCountAsync();

        // 3. Assert the count 1
        Assert.That(itemCount, Is.EqualTo(1));
        // 3. Verify the item's name
        var itemNames = await cartPage.GetCartItemNamesAsync();
        Assert.That(itemNames, Does.Contain("Sauce Labs Bike Light"));

    }

    // -------------------------------------------------------------------------
    // 🟡 Intermediate tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Test: Adding multiple products updates the badge count correctly.
    ///<para>
    /// Steps:
    ///   1. Add "Sauce Labs Backpack" to the cart.
    ///   2. Add "Sauce Labs Bike Light" to the cart.
    ///   3. Add "Sauce Labs Bolt T-Shirt" to the cart.
    ///</para>
    /// Acceptance criteria:
    ///   • The cart badge displays the number 3.
    ///   • The cart page contains all three product names.
    /// </summary>
    [Test]
    [Description("🟡 Verify the cart badge and cart contents are correct after adding multiple items.")]
    public async Task AddMultipleProducts_ShouldReflectCorrectBadgeAndCartContents()
    {
        var productsPage = new ProductsPage(Page);

        //add items
        await productsPage.AddProductToCartAsync("Sauce Labs Backpack");
        await productsPage.AddProductToCartAsync("Sauce Labs Bike Light");
        await productsPage.AddProductToCartAsync("Sauce Labs Bolt T-Shirt");

         // Navigate to the cart and capture the returned CartPage instance
        var cartPage = await productsPage.GoToCartAsync();

        var itemCount = await cartPage.GetCartItemCountAsync();

        //count that there are 3 items
        Assert.That(itemCount, Is.EqualTo(3));

         // 3. Verify the item's name
        var itemNames = await cartPage.GetCartItemNamesAsync();
        Assert.That(itemNames, Does.Contain("Sauce Labs Backpack"));
        Assert.That(itemNames, Does.Contain("Sauce Labs Bike Light"));
        Assert.That(itemNames, Does.Contain("Sauce Labs Bolt T-Shirt"));

    }

    /// <summary>
    /// Test: Removing a product from the cart updates the cart correctly.
    ///<para>
    /// Steps:
    ///   1. Add "Sauce Labs Backpack" and "Sauce Labs Bike Light" to the cart.
    ///   2. Navigate to the cart page.
    ///   3. Remove "Sauce Labs Backpack".
    ///</para>
    /// Acceptance criteria:
    ///   • The cart contains exactly 1 item.
    ///   • The remaining item is "Sauce Labs Bike Light".
    ///   • "Sauce Labs Backpack" is no longer in the cart.
    /// </summary>
    [Test]
    [Description("🟡 Verify that removing one item from a multi-item cart leaves the remaining item intact.")]
    public async Task RemoveProduct_FromCartWithMultipleItems_ShouldUpdateCartCorrectly()
    {
        var productsPage = new ProductsPage(Page);
        //add items
        await productsPage.AddProductToCartAsync("Sauce Labs Backpack");
        await productsPage.AddProductToCartAsync("Sauce Labs Bike Light");
         // Navigate to the cart and capture the returned CartPage instance
        var cartPage = await productsPage.GoToCartAsync();

        //remove item
        await cartPage.RemoveProductAsync("Sauce Labs Backpack");

        var itemCount = await cartPage.GetCartItemCountAsync();

        //count that there are 3 items
        Assert.That(itemCount, Is.EqualTo(1));

        var itemNames = await cartPage.GetCartItemNamesAsync();

        //confirm item name
        Assert.That(itemNames, Does.Contain("Sauce Labs Bike Light"));

        // Get the list of item names currently in the cart
        var cartItemNames = await cartPage.GetCartItemNamesAsync();

        //  Assert that the removed product is no longer in the list
        Assert.That(cartItemNames, Does.Not.Contain("Sauce Labs Backpack"));

    }

    /// <summary>
    /// Test: Continuing shopping from the cart returns the user to the products page.
    ///<para>
    /// Steps:
    ///   1. Add any product to the cart.
    ///   2. Navigate to the cart page.
    ///   3. Click "Continue Shopping".
    ///</para>
    /// Acceptance criteria:
    ///   • The browser URL contains "inventory".
    ///   • The products page title is visible.
    /// </summary>
    [Test]
    [Description("🟡 Verify that Continue Shopping navigates back to the inventory page.")]
    public async Task ContinueShopping_FromCart_ShouldReturnToProductsPage()
    {
        var productsPage = new ProductsPage(Page);
        //add items
        await productsPage.AddProductToCartAsync("Sauce Labs Backpack");
        //go to cart page
        var cartPage = await productsPage.GoToCartAsync();
        // Click Continue Shopping and get the returned ProductsPage
        productsPage = await cartPage.ContinueShoppingAsync();

        // Verify we returned to the Products page
        Assert.That(await productsPage.GetTitleAsync(), Is.EqualTo("Swag Labs"));
    }

    // -------------------------------------------------------------------------
    // 🟠 Advanced tests
    // -------------------------------------------------------------------------

    /// <summary>
    /// Test: Cart persists its contents after navigating away and returning.
    ///<para>
    /// Steps:
    ///   1. Add "Sauce Labs Fleece Jacket" to the cart.
    ///   2. Navigate to the cart page.
    ///   3. Click "Continue Shopping" to return to the products page.
    ///   4. Navigate to the cart page again.
    ///</para>
    /// Acceptance criteria:
    ///   • The cart still contains "Sauce Labs Fleece Jacket".
    ///   • The cart badge still shows 1.
    /// </summary>
    [Test]
    [Description("🟠 Verify that cart contents persist when navigating away and returning to the cart.")]
    public async Task Cart_ShouldPersistContents_AfterNavigatingAwayAndBack()
    {
        var productsPage = new ProductsPage(Page);
        //add items
        await productsPage.AddProductToCartAsync("Sauce Labs Fleece Jacket");
        //go to cart page
        var cartPage = await productsPage.GoToCartAsync();
        // Click Continue Shopping and get the returned ProductsPage
        productsPage = await cartPage.ContinueShoppingAsync();
        //go to cart page
        await productsPage.GoToCartAsync();
        var itemCount = await cartPage.GetCartItemCountAsync();

        //count that there are 3 items
        Assert.That(itemCount, Is.EqualTo(1));

        var itemNames = await cartPage.GetCartItemNamesAsync();

        //confirm item name
        Assert.That(itemNames, Does.Contain("Sauce Labs Fleece Jacket"));


    }

    /// <summary>
    /// Test: Each product in a data-driven set can be individually added and verified.
    ///<para>
    /// Steps:
    ///   1. For each product name supplied by the test case source:
    ///      a. Add the product to the cart.
    ///      b. Navigate to the cart.
    ///      c. Verify the product is present.
    ///      d. Remove the product to reset state for the next iteration.
    ///</para>
    /// Acceptance criteria:
    ///   • Each product name appears in the cart when added.
    ///   • The cart is empty after the product is removed.
    ///<para>
    /// Hint: The product names are already provided via [TestCaseSource] below —
    ///       use <c>_productsPage</c> to add and <c>CartPage</c> to remove.
    /// </summary>
    [Test]
    [TestCaseSource(nameof(SauceDemoProducts))]
    [Description("🟠 Verify each product can be independently added to and removed from the cart.")]
    public async Task AddAndRemoveEachProduct_ShouldMaintainCartAccuracy(string productName)
    {
        var productsPage = new ProductsPage(Page);

    // 1a. Add the product to the cart
    await productsPage.AddProductToCartAsync(productName);

    // 1b. Navigate to the cart
    var cartPage = await productsPage.GoToCartAsync();

    // 1c. Verify the product is present using your public GetCartItemNamesAsync() method
    var cartItems = await cartPage.GetCartItemNamesAsync();
    Assert.That(cartItems, Does.Contain(productName), $"Expected '{productName}' to be in the cart.");

    // 1d. Remove the product to reset state & verify cart is empty
    await cartPage.RemoveProductAsync(productName);
    var updatedCartItems = await cartPage.GetCartItemNamesAsync();
    Assert.That(updatedCartItems, Is.Empty, "Expected the cart to be empty after removing the product.");
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