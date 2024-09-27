using ORDER.API.Enums;

namespace ORDER.API.Models;

public record CreateOrderResponseModel(long Id, Guid TransactionId, string CustomerName, string CustomerEmail, long ProductId, OrderStatus Status);