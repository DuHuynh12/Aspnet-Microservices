using Basket.API;
using Basket.API.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = config.GetValue<string>("CacheSettings:ConnectionString");
    
});

// add dependency services
builder.Services.AddScoped<IBasketRepository,BasketRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();