namespace ORDER.API.Models;

public record CreateOrderRequestModel(string CustomerName, string CustomerEmail, long ProductId,Guid TransactionId);
