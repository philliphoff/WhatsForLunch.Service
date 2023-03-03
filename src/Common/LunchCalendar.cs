using System.Text.Json.Serialization;

namespace WhatsForLunch;

public sealed record LunchMenu(
    [property: JsonPropertyName("items")]
    string[] Items
);

public sealed record LunchDay(
    [property: JsonPropertyName("dateString")]
    string DateString,

    [property: JsonPropertyName("month")]
    string Month,

    [property: JsonPropertyName("month")]
    string ShortMonth,

    [property: JsonPropertyName("dayOfMonth")]
    int DayOfMonth,

    [property: JsonPropertyName("dayOfWeek")]
    string DayOfWeek,

    [property: JsonPropertyName("menu")]
    LunchMenu? Menu);

public sealed record LunchCalendar(
    [property: JsonPropertyName("today")]
    LunchDay Today,

    [property: JsonPropertyName("next")]
    LunchDay Next);
