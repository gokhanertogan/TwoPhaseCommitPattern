namespace COORDINATOR.API.Models;

public record CreateOrderDto(string CustomerName, string CustomerEmail, long ProductId, Guid TransactionId);