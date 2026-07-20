using Microsoft.Playwright;
using TestAutomation.Core.Base;

namespace TestAutomation.PageObjects.SauceDemo;

/// <summary>
/// Page Object for the SauceDemo products / inventory page (/inventory.html).
/// Encapsulates browsing, sorting, and adding items to the cart.
/// </summary>
public sealed class ProductsPage : BasePage
{
    // -------------------------------------------------------------------------
    // Locators — private
    // -------------------------------------------------------------------------

    private ILocator PageTitle        => Page.Locator(".title");
    private ILocator ProductItems     => Page.Locator(".inventory_item");
    private ILocator SortDropdown     => Page.GetByRole(AriaRole.Combobox);
    private ILocator CartBadge        => Page.Locator(".shopping_cart_badge");
    private ILocator CartIcon         => Page.Locator(".shopping_cart_link");

    private ILocator AddToCartButton(string productName) =>
        Page.GetByRole(AriaRole.Button, new() { Name = $"Add to cart" })
            .Nth(ProductIndexOf(productName));

    private ILocator ProductNameLocators => Page.Locator(".inventory_item_name");

    // -------------------------------------------------------------------------
    // Constructor
    // -------------------------------------------------------------------------

    public ProductsPage(IPage page) : base(page) { }

    // -------------------------------------------------------------------------
    // Public page actions
    // -------------------------------------------------------------------------

    /// <summary>
    /// Sorts the product list using the visible sort dropdown label,
    /// e.g. "Name (A to Z)", "Price (low to high)".
    /// </summary>
    public async Task SortByAsync(string optionLabel) =>
        await SelectByLabelAsync(SortDropdown, optionLabel);

    /// <summary>
    /// Clicks the "Add to cart" button for the product whose name
    /// matches <paramref name="productName"/> (case-sensitive).
    /// </summary>
    public async Task AddProductToCartAsync(string productName)
    {
        var button = Page.Locator(".inventory_item")
            .Filter(new LocatorFilterOptions { HasText = productName })
            .GetByRole(AriaRole.Button, new() { Name = "Add to cart" });

        await ClickAsync(button);
    }

    /// <summary>
    /// Navigates to the shopping cart page.
    /// Returns a <see cref="CartPage"/> ready for assertions.
    /// </summary>
    public async Task<CartPage> GoToCartAsync()
    {
        await ClickAsync(CartIcon);
        await WaitForUrlContainsAsync("cart");
        return new CartPage(Page);
    }

    // -------------------------------------------------------------------------
    // Public page queries
    // -------------------------------------------------------------------------

    /// <summary>Returns the heading text displayed at the top of the page.</summary>
    public async Task<string> GetPageTitleAsync() =>
        await GetTextAsync(PageTitle);

    /// <summary>Returns the number of products currently listed on the page.</summary>
    public async Task<int> GetProductCountAsync() =>
        await ProductItems.CountAsync();

    /// <summary>
    /// Returns the number shown on the cart badge icon.
    /// Returns <c>0</c> when the badge is not visible (empty cart).
    /// </summary>
    public async Task<int> GetCartBadgeCountAsync()
    {
        if (!await IsVisibleAsync(CartBadge))
            return 0;

        var text = await GetTextAsync(CartBadge);
        return int.TryParse(text, out var count) ? count : 0;
    }

    /// <summary>
    /// Returns an ordered list of all product names displayed on the page.
    /// </summary>
    public async Task<IReadOnlyList<string>> GetAllProductNamesAsync() =>
        await ProductNameLocators.AllInnerTextsAsync();

    /// <summary>
    /// Returns <c>true</c> when the products page is currently displayed.
    /// </summary>
    public async Task<bool> IsDisplayedAsync() =>
        await IsVisibleAsync(PageTitle);

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private int ProductIndexOf(string productName)
    {
        // Synchronous index lookup is intentionally avoided;
        // callers should use the filter-based overload above.
        throw new NotSupportedException(
            "Use AddProductToCartAsync(string productName) which resolves the index via Playwright filter.");
    }
}