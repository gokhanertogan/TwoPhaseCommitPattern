using System.ComponentModel.DataAnnotations;
using ORDER.API.Enums;

namespace ORDER.API.Entities;

public class Order
{
    [Key]
    public long Id { get; set; }
    public Guid TransactionId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public long ProductId { get; set; }
    public OrderStatus Status { get; set; }
}