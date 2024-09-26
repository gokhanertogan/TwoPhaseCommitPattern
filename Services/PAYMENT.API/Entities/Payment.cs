using System.ComponentModel.DataAnnotations;
using PAYMENT.API.Enums;

namespace PAYMENT.API.Entities;

public class Payment
{
    [Key]
    public long Id { get; set; }
    public Guid TransactionId { get; set; }
    public long OrderId { get; set; }
    public PaymentStatus Status { get; set; }

}