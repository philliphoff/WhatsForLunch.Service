using Microsoft.JSInterop;

public sealed class TimeZoneService
{
    private readonly IJSRuntime jsRuntime;

    private TimeSpan? userOffset;

    public TimeZoneService(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public async ValueTask<TimeSpan> GetLocalDateTime()
    {
        if (userOffset == null)
        {
            int offsetInMinutes = await jsRuntime.InvokeAsync<int>("blazorGetTimezoneOffset");
            userOffset = TimeSpan.FromMinutes(-offsetInMinutes);
        }

        return userOffset.Value;
    }
}
