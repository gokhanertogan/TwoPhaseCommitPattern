using System.ComponentModel.DataAnnotations;

namespace STOCK.API.Entities;
public class Stock
{
    [Key]
    public long Id { get; set; }

    public long ProductId { get; set; }

    public long Quantity { get; set; }
}