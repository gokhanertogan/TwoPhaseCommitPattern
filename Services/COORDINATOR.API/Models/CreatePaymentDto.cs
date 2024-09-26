namespace COORDINATOR.API.Models;

public record CreatePaymentDto(Guid TransactionId, long OrderId);