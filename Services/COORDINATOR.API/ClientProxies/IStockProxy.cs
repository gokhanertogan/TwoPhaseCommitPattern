using COORDINATOR.API.Clients;

namespace COORDINATOR.API.ClientProxies;

public interface IStockProxy
{
    Task<bool> CheckStockAsync();
    Task<bool> DecreaseStockAsync(long productId);
    Task<bool> IncreaseStockAsync(long productId);
}

public class StockProxy(StockApiClient stockApiClient) : IStockProxy
{
    private readonly StockApiClient _stockApiClient = stockApiClient;

    public async Task<bool> CheckStockAsync()
    {
        var (isSuccess, response, statusCode) = await _stockApiClient.CheckStockAsync();

        return response && isSuccess;
    }

    public async Task<bool> DecreaseStockAsync(long productId)
    {
        var (isSuccess, response, statusCode) = await _stockApiClient.DecreaseStockAsync(productId);

        return response && isSuccess;
    }

    public async Task<bool> IncreaseStockAsync(long productId)
    {
        var (isSuccess, response, statusCode) = await _stockApiClient.IncreaseStockAsync(productId);

        return response && isSuccess;
    }
}
