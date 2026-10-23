using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAYMENT.API;
using PAYMENT.API.Entities;
using PAYMENT.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IPaymentService, PaymentService>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/check-payment", async (IPaymentService paymentService) =>
{
    bool result = await paymentService.CheckPaymentAsync();
    return Results.Ok(result);
});

app.MapPost("/create-payment", async (IPaymentService paymentService, CreatePaymentRequestModel requestModel) =>
{
    if (requestModel is null)
    {
        return Results.BadRequest("payment cannot be null");
    }

    var createdPayment = await paymentService.CreatePaymentAsync(requestModel);
    return Results.Ok(createdPayment);
});

app.MapPost("/rollback-payment", async (IPaymentService paymentService, [FromBody] Guid transactionId) =>
{
    var result = await paymentService.RollbackPayment(transactionId);
    return Results.Ok(result);
});

app.Run();
