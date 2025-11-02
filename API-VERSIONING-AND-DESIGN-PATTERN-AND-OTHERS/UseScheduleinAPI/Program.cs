using Coravel;
using UseScheduleinAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Register Coravel services
builder.Services.AddScheduler();
// Register your job
builder.Services.AddTransient<MyScheduledJob>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Services.UseScheduler(scheduler =>
{
    scheduler
        .Schedule<MyScheduledJob>()
        .EveryMinute(); // or .Hourly(), .Daily(), etc.
});

app.Run();
