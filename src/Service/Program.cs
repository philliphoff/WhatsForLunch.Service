using System.Text.Json.Serialization;

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
}

app.MapGet(
        "/todayslunch",
        () =>
        {
            return new LunchCalendar(
                new LunchDay(
                    "",
                    "",
                    new LunchMenu()),
                new LunchDay(
                    "",
                    "",
                    new LunchMenu()));
        })
    .WithName("Today's Lunch")
    .WithOpenApi();

app.Run();

sealed record LunchMenu();

sealed record LunchDay(
    [property: JsonPropertyName("dateString")]
    string DateString,

    [property: JsonPropertyName("dayOfWeek")]
    string DayOfWeek,

    [property: JsonPropertyName("menu")]
    LunchMenu? Menu);

sealed record LunchCalendar(
    [property: JsonPropertyName("today")]
    LunchDay Today,

    [property: JsonPropertyName("next")]
    LunchDay Next);
