using System.Text;
using System.Text.Json;
using COORDINATOR.API.ClientProxies;
using COORDINATOR.API.Entities;
using COORDINATOR.API.Enums;
using COORDINATOR.API.Models;
using Microsoft.EntityFrameworkCore;

namespace COORDINATOR.API;

public interface ITransactionService
{
    public Task<Guid> CreateTransactionAsync();
    public Task PrepareServicesAsync(Guid transactionId);
    public Task<bool> CheckReadyServicesAsync(Guid transactionId);
    public Task<bool> CheckTransactionStateServicesAsync(Guid transactionId);
    public Task CommitAsync(Guid transactionId, CreateOrderRequestModel requestModel);
    public Task RollbackAsync(Guid transactionId);
}

public class TransactionService(ApplicationDbContext dbContext, IOrderProxy orderProxy, IPaymentProxy paymentProxy, IStockProxy stockProxy) : ITransactionService
{
    private readonly IOrderProxy _orderProxy = orderProxy;
    private readonly IPaymentProxy _paymentProxy = paymentProxy;
    private readonly IStockProxy _stockProxy = stockProxy;

    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<bool> CheckReadyServicesAsync(Guid transactionId) =>
       (await _dbContext.NodeStates
               .Where(ns => ns.TransactionId == transactionId)
               .ToListAsync())
       .TrueForAll(n => n.IsReady == ReadyType.Ready);

    public async Task<bool> CheckTransactionStateServicesAsync(Guid transactionId) =>
        (await _dbContext.NodeStates
                .Where(ns => ns.TransactionId == transactionId)
                .ToListAsync())
        .TrueForAll(n => n.TransactionState == TransactionState.Done);

    public async Task CommitAsync(Guid transactionId, CreateOrderRequestModel requestModel)
    {
        NodeState transactionNode = null!;
        CreateOrderResponseModel createdOrder = null!;
        bool result = false;

        var transactionNodes = await _dbContext.NodeStates
           .Include(ns => ns.Node)
           .Where(ns => ns.TransactionId == transactionId)
           .ToListAsync();

        var nodes = transactionNodes.Select(x => x.Node).ToList();

        var orderedLogicalCommits = new Dictionary<string, string>
        {
            { "Order.API", "create-order" },
            { "Payment.API", "create-payment" },
            { "Stock.API", "decrease-stock" }
        };

        var createOrderRequestDto = new CreateOrderDto(requestModel.CustomerName, requestModel.CustomerEmail, requestModel.ProductId, transactionId);

        foreach (var orderedLogicalCommit in orderedLogicalCommits)
        {
            result = false;
            try
            {
                var node = nodes.FirstOrDefault(x => x?.Name == orderedLogicalCommit.Key);
                transactionNode = transactionNodes.FirstOrDefault(x => x.Node == node)!;

                if (orderedLogicalCommit.Key == "ORDER.API")
                {
                    createdOrder = await _orderProxy.CreateOrderAsync(createOrderRequestDto);

                    if (createdOrder is not null)
                    {
                        await _dbContext.AddAsync(new TransactionData()
                        {
                            TransactionId = transactionId,
                            RequestData = JsonSerializer.Serialize(createdOrder)
                        });

                        result = true;
                    }
                }

                else if (orderedLogicalCommit.Key == "Payment.API" && createdOrder is not null)
                {
                    result = await _paymentProxy.CreatePaymentAsync(new CreatePaymentDto(transactionId, createdOrder.Id));
                }

                else if (orderedLogicalCommit.Key == "Stock.API" && createdOrder is not null)
                {
                    result = await _stockProxy.DecreaseStockAsync(createdOrder.ProductId);
                }

                transactionNode.TransactionState = result ? TransactionState.Done : TransactionState.Abort;
            }

            catch
            {
                if (transactionNode is not null)
                {
                    transactionNode.TransactionState = TransactionState.Abort;
                }
            }
        }

        await _dbContext.SaveChangesAsync();

        if (transactionNodes.All(x => x.TransactionState == TransactionState.Done) && createdOrder is not null)
        {
            await _orderProxy.CompleteOrderAsync(createdOrder.Id);
        }
    }

    public async Task<Guid> CreateTransactionAsync()
    {
        Guid transactionId = Guid.NewGuid();
        var nodes = await _dbContext.Nodes.ToListAsync();
        nodes.ForEach(node => node.NodeStates = new List<NodeState>
        {
            new()
            {
                TransactionId = transactionId,
                IsReady = ReadyType.Pending,
                TransactionState = TransactionState.Pending
            },
        });

        await _dbContext.SaveChangesAsync();
        return transactionId;
    }

    public async Task PrepareServicesAsync(Guid transactionId)
    {
        var transactionNodes = await _dbContext.NodeStates
           .Include(ns => ns.Node)
           .Where(n => n.TransactionId == transactionId)
           .ToListAsync();

        foreach (var transactionNode in transactionNodes)
        {
            try
            {
                var result = transactionNode?.Node?.Name switch
                {
                    "Order.API" => await _orderProxy.CheckOrderAsync(),
                    "Stock.API" => await _stockProxy.CheckStockAsync(),
                    "Payment.API" => await _paymentProxy.CheckPaymentAsync(),
                    _ => throw new NotImplementedException()
                };

                await Console.Out.WriteLineAsync(result.ToString());
                transactionNode.IsReady = result ? ReadyType.Ready : ReadyType.Unready;
            }
            catch
            {
                transactionNode.IsReady = ReadyType.Unready;
            }
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task RollbackAsync(Guid transactionId)
    {
        var transactionNodes = await _dbContext.NodeStates
           .Include(ns => ns.Node)
           .Where(n => n.TransactionId == transactionId)
           .ToListAsync();

        var transactionData = await _dbContext.TransactionDatas.FirstOrDefaultAsync(x => x.TransactionId == transactionId);
        var createdOrder = JsonSerializer.Deserialize<CreateOrderDto>(transactionData!.RequestData!);

        transactionNodes.ForEach(async transactionNode =>
        {
            try
            {
                if (transactionNode.TransactionState == TransactionState.Done)
                    _ = transactionNode?.Node?.Name switch
                    {
                        "Order.API" => await _orderProxy.RollbackOrderAsync(createdOrder!.TransactionId),
                        "Stock.API" => await _stockProxy.IncreaseStockAsync(createdOrder!.ProductId),
                        "Payment.API" => await _paymentProxy.RollbackPayment(createdOrder!.TransactionId),
                        _ => throw new NotImplementedException(),
                    };
                transactionNode.TransactionState = TransactionState.Abort;
            }
            catch
            {
                transactionNode.TransactionState = TransactionState.Abort;
            }
        });

        await _dbContext.SaveChangesAsync();
    }
}