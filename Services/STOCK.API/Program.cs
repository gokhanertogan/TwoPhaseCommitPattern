using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STOCK.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IStockService, StockService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/check-stock", async (IStockService stockService) =>
{
    bool result = await stockService.CheckStockAsync();
    return Results.Ok(result);
});

app.MapPost("/decrease-stock/{productId}", async (IStockService stockService, [FromRoute] int productId) =>
{
    var result = await stockService.DecreaseStockAsync(productId);
    return Results.Ok(result);
});

app.MapPost("/increase-stock/{productId}", async (IStockService stockService, [FromRoute] int productId) =>
{
    var result = await stockService.IncreaseStockAsync(productId);
    return Results.Ok(result);
});

app.Run();

