namespace COORDINATOR.API.Models;

public record CreateOrderResponseModel(long Id, Guid TransactionId, string CustomerName, string CustomerEmail, long ProductId, string Status);