using Microsoft.AspNetCore.Mvc;
using WhatsForLunch;
using WhatsForLunch.Service.Titan;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseWebAssemblyDebugging();
}

app.UseRouting();
app.UseStaticFiles();
app.UseBlazorFrameworkFiles();
app.UseEndpoints(
    endpoints =>
    {
        endpoints.MapFallbackToFile("index.html");
    });

app.MapGet(
        "api/v1/todayslunch",
        async ([FromQuery] DateOnly? today, [FromServices] IConfiguration configuration) =>
        {
            today ??= DateOnly.FromDateTime(DateTime.Today);

            string? endpoint = configuration["TITAN_MENU_ENDPOINT"];

            if (String.IsNullOrEmpty(endpoint))
            {
                throw new InvalidOperationException("TITAN_MENU_ENDPOINT not set.");
            }

            var menuClient = new TitanMenuClient(new Uri(endpoint));

            string? identifier = configuration["TITAN_MENU_IDENTIFIER"];

            if (String.IsNullOrEmpty(identifier))
            {
                throw new InvalidOperationException("TITAN_MENU_IDENTIFIER not set.");
            }

            var menuIdentifiers = await menuClient.GetMenuIdentifiersAsync(identifier);

            if (menuIdentifiers?.DistrictId == null)
            {
                throw new InvalidOperationException("Unable to determine the school district ID.");
            }

            if (menuIdentifiers?.SelectedBuildingId == null)
            {
                throw new InvalidOperationException("Unable to determine the school building ID.");
            }

            var calendarDates = GetCalendarDates(today.Value);
            var weekStartDates = GetWeekStartDates(calendarDates.Today, calendarDates.Next);

            var lunchMenuDays = new Dictionary<DateOnly, Day>();

            foreach (var weekStartDate in weekStartDates)
            {
                var days = await GetLunchMenuDaysAsync(menuClient, menuIdentifiers.DistrictId, menuIdentifiers.SelectedBuildingId, weekStartDate);

                foreach (var day in days)
                {
                    lunchMenuDays.Add(day.Key, day.Value);
                }
            }

            return new LunchCalendar(
                ToLunchDay(calendarDates.Today, lunchMenuDays),
                ToLunchDay(calendarDates.Next, lunchMenuDays));
        })
    .WithName("Today's Lunch")
    .WithOpenApi();

app.Run();

LunchMenu ToLunchMenu(Day day)
{
    return new LunchMenu(
        day
            .RecipeCategories
            .SelectMany(category => category.Recipes)
            .Select(recipe => recipe.RecipeName ?? String.Empty)
            .Where(recipeName => !String.IsNullOrEmpty(recipeName))
            .ToArray()
    );
}

LunchDay ToLunchDay(DateOnly date, IReadOnlyDictionary<DateOnly, Day> lunchMenuDays)
{
    return new LunchDay(
        date.ToString(),
        date.DayOfWeek.ToString(), // TODO: Find culture-appropriate conversion.
        lunchMenuDays.TryGetValue(date, out var day)
            ? ToLunchMenu(day)
            : null);
}

(DateOnly Today, DateOnly Next) GetCalendarDates(DateOnly today)
{
    return today.DayOfWeek switch {
        // Sunday through Thursday show today and tomorrow...
        DayOfWeek.Sunday
            or DayOfWeek.Monday
            or DayOfWeek.Tuesday
            or DayOfWeek.Wednesday
            or DayOfWeek.Thursday => (today, today.AddDays(1)),

        // Friday show today and Monday...
        DayOfWeek.Friday => (today, today.AddDays(3)),

        // Saturday show today and Monday...
        DayOfWeek.Saturday => (today, today.AddDays(2)),
        _ => throw new ArgumentOutOfRangeException(nameof(today))
    };
}

IReadOnlySet<DateOnly> GetWeekStartDates(params DateOnly[] calendarDates)
{
    return calendarDates.Select(date => date.AddDays(-(int)date.DayOfWeek)).ToHashSet();
}

async Task<IReadOnlyDictionary<DateOnly, Day>> GetLunchMenuDaysAsync(TitanMenuClient menuClient, string districtId, string buildingId, DateOnly startDate)
{
    var menus = await menuClient.GetMenusAsync(
        districtId,
        buildingId,
        startDate);

    var lunchMenuSession = menus?.FamilyMenuSessions.FirstOrDefault(session => session.ServingSession == "Lunch");
    var lunchMenuPlan = lunchMenuSession?.MenuPlans.FirstOrDefault();

    if (lunchMenuPlan == null)
    {
        return new Dictionary<DateOnly, Day>();
    }

    return lunchMenuPlan
        .Days
        .Where(day => day.Date != null)
        .ToDictionary(day => DateOnly.ParseExact(day.Date!, "M/d/yyyy"));
}
