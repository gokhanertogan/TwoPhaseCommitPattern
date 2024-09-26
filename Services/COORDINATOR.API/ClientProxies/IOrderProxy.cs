using COORDINATOR.API.Clients;
using COORDINATOR.API.Models;

namespace COORDINATOR.API.ClientProxies;

public interface IOrderProxy
{
    Task<bool> CheckOrderAsync();
    Task<CreateOrderResponseModel> CreateOrderAsync(CreateOrderDto requestModel);
    Task<bool> RollbackOrderAsync(Guid transactionId);
    Task<bool> CompleteOrderAsync(long orderId);
}

public class OrderProxy(OrderApiClient orderApiClient) : IOrderProxy
{
    private readonly OrderApiClient _orderApiClient = orderApiClient;

    public async Task<bool> CheckOrderAsync()
    {
        var (isSuccess, response, statusCode) = await _orderApiClient.CheckOrderAsync();

        return response && isSuccess;
    }

    public async Task<bool> CompleteOrderAsync(long orderId)
    {
        return await _orderApiClient.CompleteOrderAsync(orderId);
    }

    public async Task<CreateOrderResponseModel> CreateOrderAsync(CreateOrderDto requestModel)
    {
        var (isSuccess, response, statusCode) = await _orderApiClient.CreateOrderAsync(requestModel);
        if (!isSuccess)
        {
            return null!;
        }
        return response!;
    }

    public async Task<bool> RollbackOrderAsync(Guid transactionId)
    {
        return await _orderApiClient.RollbackOrderAsync(transactionId);
    }
}