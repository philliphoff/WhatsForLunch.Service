using System.Text.Json.Serialization;

namespace WhatsForLunch.Service.Titan;

internal sealed record TitanMenuIdentifiers
{
    [JsonPropertyName("DistrictId")]
    public string? DistrictId { get; init; }

    [JsonPropertyName("DistrictName")]
    public string? DistrictName { get; init; }

    [JsonPropertyName("Buildings")]
    public TitanBuilding[] Buildings { get; init; } = Array.Empty<TitanBuilding>();

    [JsonPropertyName("SelectedBuildingId")]
    public string? SelectedBuildingId { get; init; }
}

internal sealed record TitanBuilding
{
    [JsonPropertyName("BuildingId")]
    public string? BuildingId { get; init; }

    [JsonPropertyName("Name")]
    public string? Name { get; init; }
}
