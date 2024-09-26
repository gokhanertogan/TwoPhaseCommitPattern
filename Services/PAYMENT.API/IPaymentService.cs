using Microsoft.EntityFrameworkCore;
using PAYMENT.API.Entities;
using PAYMENT.API.Enums;
using PAYMENT.API.Models;

namespace PAYMENT.API;

public interface IPaymentService
{
    Task<bool> CheckPaymentAsync();
    Task<bool> CreatePaymentAsync(CreatePaymentRequestModel requestModel);
    Task<bool> RollbackPayment(Guid transactionId);
}

public class PaymentService(ApplicationDbContext dbContext) : IPaymentService
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<bool> CheckPaymentAsync()
    {
        return Task.FromResult(true);
    }

    public async Task<bool> CreatePaymentAsync(CreatePaymentRequestModel requestModel)
    {
        try
        {
            var payment = new Payment()
            {
                OrderId = requestModel.OrderId,
                TransactionId = requestModel.TransactionId,
                Status = PaymentStatus.Complete,
            };

            await _dbContext.Payments.AddAsync(payment);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RollbackPayment(Guid transactionId)
    {
        var payment = await _dbContext.Payments.FirstOrDefaultAsync(x => x.TransactionId == transactionId);
        if (payment == null)
            return false;

        payment.Status = PaymentStatus.Cancel;

        _dbContext.Update(payment);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}