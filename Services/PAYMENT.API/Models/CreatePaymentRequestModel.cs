namespace PAYMENT.API.Models;

public record CreatePaymentRequestModel(Guid TransactionId, long OrderId);