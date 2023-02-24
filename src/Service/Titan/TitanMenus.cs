using System.Text.Json.Serialization;

namespace WhatsForLunch.Service.Titan;

internal sealed record TitanMenus
{
    [JsonPropertyName("FamilyMenuSessions")]
    public FamilyMenuSession[] FamilyMenuSessions { get; init; } = Array.Empty<FamilyMenuSession>();

    [JsonPropertyName("AcademicCalendars")]
    public object[] AcademicCalendars { get; init; } = Array.Empty<object>();
}

internal sealed record FamilyMenuSession
{
    [JsonPropertyName("ServingSession")]
    public string? ServingSession { get; init; }

    [JsonPropertyName("MenuPlans")]
    public MenuPlan[] MenuPlans { get; init; } = Array.Empty<MenuPlan>();
}

internal sealed record MenuPlan
{
    [JsonPropertyName("MenuPlanName")]
    public string? MenuPlanName { get; init; }

    [JsonPropertyName("Days")]
    public Day[] Days { get; init; } = Array.Empty<Day>();

    [JsonPropertyName("AcademicCalenderId")]
    public string? AcademicCalenderId { get; init; }
}

internal sealed record class Day
{
    [JsonPropertyName("Date")]
    public string? Date { get; init; }

    [JsonPropertyName("RecipeCategories")]
    public RecipeCategory[] RecipeCategories { get; init; } = Array.Empty<RecipeCategory>();
}

internal sealed record RecipeCategory
{
    [JsonPropertyName("CategoryName")]
    public string? CategoryName { get; init; }

    [JsonPropertyName("Color")]
    public string? Color { get; init; }

    [JsonPropertyName("Recipes")]
    public Recipe[] Recipes { get; init; } = Array.Empty<Recipe>();
}

internal sealed record Recipe
{
    [JsonPropertyName("ItemId")]
    public string? ItemId { get; init; }

    [JsonPropertyName("RecipeIdentifier")]
    public string? RecipeIdentifier { get; init; }

    [JsonPropertyName("RecipeName")]
    public string? RecipeName { get; init; }

    [JsonPropertyName("ServingSize")]
    public string? ServingSize { get; init; }

    [JsonPropertyName("GramPerServing")]
    public double GramPerServing { get; init; }

    [JsonPropertyName("Nutrients")]
    public object[] Nutrients { get; init; } = Array.Empty<object>();

    [JsonPropertyName("Allergens")]
    public string[] Allergens { get; init; } = Array.Empty<string>();

    [JsonPropertyName("ReligiousRestrictions")]
    public object[] ReligiousRestrictions { get; init; } = Array.Empty<object>();

    [JsonPropertyName("DietaryRestrictions")]
    public object[] DietaryRestrictions { get; init; } = Array.Empty<object>();

    [JsonPropertyName("HasNutrients")]
    public bool? HasNutrients { get; init; }
}
