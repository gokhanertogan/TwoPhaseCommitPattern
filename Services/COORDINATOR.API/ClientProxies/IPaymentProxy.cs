using COORDINATOR.API.Clients;
using COORDINATOR.API.Models;

namespace COORDINATOR.API.ClientProxies;

public interface IPaymentProxy
{
    Task<bool> CheckPaymentAsync();
    Task<bool> CreatePaymentAsync(CreatePaymentDto payment);
    Task<bool> RollbackPayment(Guid transactionId);
}

public class PaymentProxy(PaymentApiClient paymentApiClient) : IPaymentProxy
{
    private readonly PaymentApiClient _paymentApiClient = paymentApiClient;

    public async Task<bool> CheckPaymentAsync()
    {
        var (isSuccess, response, statusCode) = await _paymentApiClient.CheckPaymentAsync();

        return response && isSuccess;
    }

    public async Task<bool> CreatePaymentAsync(CreatePaymentDto payment)
    {
        var (isSuccess, response, statusCode) = await _paymentApiClient.CreatePaymentAsync(payment);

        return response && isSuccess;
    }

    public async Task<bool> RollbackPayment(Guid transactionId)
    {
        return await _paymentApiClient.RollbackPaymentAsync(transactionId);
    }
}
