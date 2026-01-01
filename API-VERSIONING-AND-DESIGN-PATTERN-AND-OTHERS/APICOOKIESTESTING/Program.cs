using APICOOKIESTESTING;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<NotificationHub>("/hubs/notification");

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Frontend");
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<CookiesMiddleware>();

app.MapControllers();

app.UseCookieValidation();

app.MapGet("/api/data", () =>
{
    return Results.Ok(new { Message = "Cookie Validated! Access granted." });
});

app.Run();

app.Run();
