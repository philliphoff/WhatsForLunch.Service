namespace WhatsForLunch;

public interface ILunchCalendarService
{
    Task<LunchCalendar> GetTodaysLunch(DateOnly today);
}
