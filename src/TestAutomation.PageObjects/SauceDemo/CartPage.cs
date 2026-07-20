using Microsoft.Playwright;
using TestAutomation.Core.Base;

namespace TestAutomation.PageObjects.SauceDemo;

/// <summary>
/// Page Object for the SauceDemo shopping cart page (/cart.html).
/// Encapsulates cart item inspection and checkout initiation.
/// </summary>
public sealed class CartPage : BasePage
{
    // -------------------------------------------------------------------------
    // Locators — private
    // -------------------------------------------------------------------------

    private ILocator CartItems          => Page.Locator(".cart_item");
    private ILocator CartItemNames      => Page.Locator(".inventory_item_name");
    private ILocator CartItemPrices     => Page.Locator(".inventory_item_price");
    private ILocator CheckoutButton     => Page.GetByRole(AriaRole.Button, new() { Name = "Checkout" });
    private ILocator ContinueShoppingButton => Page.GetByRole(AriaRole.Button, new() { Name = "Continue Shopping" });

    private ILocator RemoveButtonFor(string productName) =>
        Page.Locator(".cart_item")
            .Filter(new LocatorFilterOptions { HasText = productName })
            .GetByRole(AriaRole.Button, new() { Name = "Remove" });

    // -------------------------------------------------------------------------
    // Constructor
    // -------------------------------------------------------------------------

    public CartPage(IPage page) : base(page) { }

    // -------------------------------------------------------------------------
    // Public page actions
    // -------------------------------------------------------------------------

    /// <summary>
    /// Removes the cart item whose name matches <paramref name="productName"/>.
    /// </summary>
    public async Task RemoveProductAsync(string productName) =>
        await ClickAsync(RemoveButtonFor(productName));

    /// <summary>
    /// Clicks the Checkout button to begin the checkout flow.
    /// </summary>
    public async Task ProceedToCheckoutAsync()
    {
        await ClickAsync(CheckoutButton);
        await WaitForUrlContainsAsync("checkout-step-one");
    }

    /// <summary>
    /// Clicks Continue Shopping to return to the products page.
    /// Returns a <see cref="ProductsPage"/> ready for further actions.
    /// </summary>
    public async Task<ProductsPage> ContinueShoppingAsync()
    {
        await ClickAsync(ContinueShoppingButton);
        await WaitForUrlContainsAsync("inventory");
        return new ProductsPage(Page);
    }

    // -------------------------------------------------------------------------
    // Public page queries
    // -------------------------------------------------------------------------

    /// <summary>Returns the number of line items currently in the cart.</summary>
    public async Task<int> GetCartItemCountAsync() =>
        await CartItems.CountAsync();

    /// <summary>
    /// Returns an ordered list of product names in the cart.
    /// </summary>
    public async Task<IReadOnlyList<string>> GetCartItemNamesAsync() =>
        await CartItemNames.AllInnerTextsAsync();

    /// <summary>
    /// Returns <c>true</c> when a product with the given name is present in the cart.
    /// </summary>
    public async Task<bool> ContainsProductAsync(string productName)
    {
        var names = await GetCartItemNamesAsync();
        return names.Any(n => n.Equals(productName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Returns an ordered list of price strings for all cart items,
    /// e.g. <c>["$29.99", "$9.99"]</c>.
    /// </summary>
    public async Task<IReadOnlyList<string>> GetCartItemPricesAsync() =>
        await CartItemPrices.AllInnerTextsAsync();

    /// <summary>
    /// Returns <c>true</c> when the cart page is currently displayed.
    /// </summary>
    public async Task<bool> IsDisplayedAsync() =>
        await IsVisibleAsync(CheckoutButton);

    /// <summary>
    /// Returns <c>true</c> when the cart contains no items.
    /// </summary>
    public async Task<bool> IsEmptyAsync() =>
        await GetCartItemCountAsync() == 0;
}