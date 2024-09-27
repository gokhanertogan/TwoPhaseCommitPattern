using System.Net.Http.Headers;
using COORDINATOR.API;
using COORDINATOR.API.ClientProxies;
using COORDINATOR.API.Clients;
using COORDINATOR.API.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<OrderApiClient>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:5000/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    });

builder.Services.AddScoped<OrderApiClient>();

builder.Services.AddHttpClient<PaymentApiClient>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:5001/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    });

builder.Services.AddScoped<PaymentApiClient>();

builder.Services.AddHttpClient<StockApiClient>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:5002/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    });

builder.Services.AddScoped<StockApiClient>();

builder.Services.AddScoped<IOrderProxy, OrderProxy>();
builder.Services.AddScoped<IPaymentProxy, PaymentProxy>();
builder.Services.AddScoped<IStockProxy, StockProxy>();

builder.Services.AddTransient<ITransactionService, TransactionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/create-order-transaction", async (ITransactionService transactionService, CreateOrderRequestModel request) =>
{
    // Phase 1 - Prepare
    var transactionId = await transactionService.CreateTransactionAsync();
    await transactionService.PrepareServicesAsync(transactionId);
    bool transactionState = await transactionService.CheckReadyServicesAsync(transactionId);

    if (transactionState)
    {
        // Phase 2 - Commit
        await transactionService.CommitAsync(transactionId, request);
        transactionState = await transactionService.CheckTransactionStateServicesAsync(transactionId);
    }

    if (!transactionState)
        await transactionService.RollbackAsync(transactionId);

    return Results.Ok(new { TransactionId = transactionId, State = transactionState });
})
.WithName("CreateOrderTransaction")
.WithOpenApi();



app.Run();
