using StratasicDesignPattern;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPayment,BkashPayment>();
builder.Services.AddScoped<IPayment,RocketPayment>();
builder.Services.AddScoped<IPayment,NagadPayment>();
builder.Services.AddScoped<IPayment,CreditCardPayment>();

builder.Services.AddScoped<IMakePayment,MakePayment>();

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

app.Run();
