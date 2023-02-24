using System.Text.Json;

namespace WhatsForLunch.Service.Titan;

internal sealed class TitanMenuClient
{
    private static HttpClient Client = new HttpClient();

    private readonly Uri endpoint;

    public TitanMenuClient(Uri endpoint)
    {
        this.endpoint = endpoint;
    }

    public async Task<TitanMenuIdentifiers?> GetMenuIdentifiersAsync(string identifier, CancellationToken cancellationToken = default)
    {
        var uri = new Uri(this.endpoint, $"FamilyMenuIdentifier?identifier={identifier}");

        string identifiersJson = await Client.GetStringAsync(uri, cancellationToken);

        return JsonSerializer.Deserialize<TitanMenuIdentifiers>(identifiersJson);
    }

    public async Task<TitanMenus?> GetMenusAsync(string districtId, string buildingId, DateOnly startDate, CancellationToken cancellationToken = default)
    {
        var uri = new Uri(this.endpoint, $"FamilyMenu?districtId={districtId}&buildingId={buildingId}&startDate={startDate.ToString("M-d-yyyy")}");

        string identifiersJson = await Client.GetStringAsync(uri, cancellationToken);

        return JsonSerializer.Deserialize<TitanMenus>(identifiersJson);
    }
}
