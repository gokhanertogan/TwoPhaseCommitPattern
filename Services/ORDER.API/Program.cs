using Microsoft.EntityFrameworkCore;
using ORDER.API;
using ORDER.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IOrderService, OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/check-order", async (IOrderService orderService) =>
{
    bool result = await orderService.CheckOrderAsync();
    return Results.Ok(result);
});

app.MapPost("/create-order", async (IOrderService orderService, CreateOrderRequestModel requestModel) =>
{
    if (requestModel is null)
    {
        return Results.BadRequest("Order cannot be null");
    }

    var createdOrder = await orderService.CreateOrderAsync(requestModel);
    return Results.Ok(createdOrder);
});

app.MapPost("/rollback-order", async (IOrderService orderService, Guid transactionId) =>
{
    var result = await orderService.RollbackOrder(transactionId);
    return Results.Ok(result);
});

app.MapPut("/complete-order", async (IOrderService orderService, int orderId) =>
{
    var result = await orderService.CompleteOrderAsync(orderId);
    return Results.Ok(result);
});

app.Run();