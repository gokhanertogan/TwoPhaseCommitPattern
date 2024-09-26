using System.ComponentModel.DataAnnotations;

namespace COORDINATOR.API.Entities;

public class TransactionData
{
    [Key]
    public int Id { get; set; }
    public Guid TransactionId { get; set; }
    public string? RequestData { get; set; }
}