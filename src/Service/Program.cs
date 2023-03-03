using Microsoft.AspNetCore.Mvc;
using WhatsForLunch;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<ILunchCalendarService, LunchCalendarService>();

builder.WebHost.UseStaticWebAssets();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapGet(
        "api/v1/todayslunch",
        ([FromQuery] DateOnly? today, [FromServices] ILunchCalendarService lunchCalendarService) =>
        {
            return lunchCalendarService.GetTodaysLunch(today);
        })
    .WithName("Today's Lunch")
    .WithOpenApi();

app.Run();
