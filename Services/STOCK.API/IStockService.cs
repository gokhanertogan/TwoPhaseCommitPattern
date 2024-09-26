using Microsoft.EntityFrameworkCore;
using STOCK.API.Entities;

namespace STOCK.API;

public interface IStockService
{
    Task<bool> CheckStockAsync();
    Task<bool> DecreaseStockAsync(int productId);
    Task<bool> IncreaseStockAsync(int productId);
}

public class StockService(ApplicationDbContext dbContext) : IStockService
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<bool> CheckStockAsync()
    {
        return Task.FromResult(true);
    }

    public async Task<bool> IncreaseStockAsync(int productId)
    {
        var stock = await _dbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == productId);
        if (stock == null)
            return false;

        stock.Quantity++;
        _dbContext.Stocks.Update(stock);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DecreaseStockAsync(int productId)
    {
        var stock = await _dbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == productId);
        if (stock == null)
            return  false;

        stock.Quantity--;
        _dbContext.Stocks.Update(stock);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}