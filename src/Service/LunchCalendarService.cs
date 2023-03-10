using WhatsForLunch.Service.Titan;

namespace WhatsForLunch;

public sealed class LunchCalendarService : ILunchCalendarService
{
    private readonly IConfiguration configuration;
    private readonly ILunchMenuNormalizationService normalizationService;

    public LunchCalendarService(
        IConfiguration configuration,
        ILunchMenuNormalizationService normalizationService)
    {
        this.configuration = configuration;
        this.normalizationService = normalizationService;
    }

    public async Task<LunchCalendar> GetTodaysLunch(DateOnly today)
    {
            string? endpoint = this.configuration["TITAN_MENU_ENDPOINT"];

            if (String.IsNullOrEmpty(endpoint))
            {
                throw new InvalidOperationException("TITAN_MENU_ENDPOINT not set.");
            }

            var menuClient = new TitanMenuClient(new Uri(endpoint));

            string? identifier = this.configuration["TITAN_MENU_IDENTIFIER"];

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

            var calendarDates = GetCalendarDates(today);
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
    }

    private LunchMenu ToLunchMenu(Day day)
    {
        return new LunchMenu(
            day
                .RecipeCategories
                .SelectMany(category => category.Recipes)
                .Select(recipe => recipe.RecipeName ?? String.Empty)
                .Where(recipeName => !String.IsNullOrEmpty(recipeName))
                .Select(item => this.normalizationService.NormalizeItem(item))
                .ToArray()
        );
    }

    private LunchDay ToLunchDay(DateOnly date, IReadOnlyDictionary<DateOnly, Day> lunchMenuDays)
    {
        return new LunchDay(
            DateString: date.ToString(),
            Month: date.ToString("MMMM"),
            ShortMonth: date.ToString("MMM"),
            DayOfMonth: date.Day,
            DayOfWeek: date.DayOfWeek.ToString(), // TODO: Find culture-appropriate conversion.
            Menu: lunchMenuDays.TryGetValue(date, out var day)
                ? ToLunchMenu(day)
                : null);
    }

    private static (DateOnly Today, DateOnly Next) GetCalendarDates(DateOnly today)
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

    private static IReadOnlySet<DateOnly> GetWeekStartDates(params DateOnly[] calendarDates)
    {
        return calendarDates.Select(date => date.AddDays(-(int)date.DayOfWeek)).ToHashSet();
    }

    private static async Task<IReadOnlyDictionary<DateOnly, Day>> GetLunchMenuDaysAsync(TitanMenuClient menuClient, string districtId, string buildingId, DateOnly startDate)
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
}
