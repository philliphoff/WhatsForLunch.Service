namespace WhatsForLunch;

public sealed class StaticLunchMenuNormalizationService : ILunchMenuNormalizationService
{
    private readonly IReadOnlyDictionary<string, string> replacements = new Dictionary<string, string>
    {
        { "1% MILK", "1% Milk" },
        { "Beans, Black", "Black Beans" },
        { "Bun, Hamburger", "Hamburger Bun" },
        { "Cherry Tomato, raw", "Cherry Tomatoes" },
        { "CHICKEN, WING", "Chicken Wing" },
        { "FAT FREE CHOC MILK", "Fat-Free Chocolate Milk" },
        { "FRENCH-FRY", "French Fries" },
        { "Pinwheels, Turkey Pepperoni", "Turkey & Pepperoni Pinwheels" },
        { "Roll, Dinner 2 oz", "Dinner Roll" },
        { "Seasonal  Veggie", "Seasonal Veggies" },
        { "Tamale, Cheese", "Cheese Tamale" }
    };

    #region ILunchMenuNormalizationService Members

    public string NormalizeItem(string item)
    {
        return this.replacements.TryGetValue(item, out string? replacement)
            ? replacement
            : item;
    }

    #endregion
}
