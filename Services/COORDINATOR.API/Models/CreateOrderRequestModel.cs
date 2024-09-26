namespace COORDINATOR.API.Models;

public record CreateOrderRequestModel(string CustomerName, string CustomerEmail, long ProductId);