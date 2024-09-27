using Microsoft.EntityFrameworkCore;
using ORDER.API.Entities;
using ORDER.API.Enums;
using ORDER.API.Models;

namespace ORDER.API;

public interface IOrderService
{
    Task<bool> CheckOrderAsync();
    Task<CreateOrderResponseModel> CreateOrderAsync(CreateOrderRequestModel order);
    Task<bool> RollbackOrder(Guid transactionId);
    Task<bool> CompleteOrderAsync(long orderId);
}

public class OrderService(ApplicationDbContext dbContext) : IOrderService
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<bool> CheckOrderAsync()
    {
        return Task.FromResult(true);
    }

    public async Task<CreateOrderResponseModel> CreateOrderAsync(CreateOrderRequestModel requestModel)
    {
        var order = new Order()
        {
            TransactionId = requestModel.TransactionId,
            CustomerEmail = requestModel.CustomerEmail,
            CustomerName = requestModel.CustomerName,
            ProductId = requestModel.ProductId,
            Status = OrderStatus.Processing
        };

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        return new CreateOrderResponseModel(order.Id,order.TransactionId,order.CustomerName,order.CustomerEmail,order.ProductId,order.Status);
    }

    public async Task<bool> CompleteOrderAsync(long orderId)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (order == null)
            return false;

        order.Status = OrderStatus.Complete;
        _dbContext.Update(order);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RollbackOrder(Guid transactionId)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.TransactionId == transactionId);
        if (order == null)
            return false;

        order.Status = OrderStatus.Fail;

        _dbContext.Update(order);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}