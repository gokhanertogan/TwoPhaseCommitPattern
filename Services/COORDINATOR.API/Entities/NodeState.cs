using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using COORDINATOR.API.Enums;

namespace COORDINATOR.API.Entities;

public class NodeState
{
    [Key]
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public ReadyType IsReady { get; set; }
    public TransactionState TransactionState { get; set; }
    [ForeignKey("Node")]
    public Guid NodeId { get; set; }
    public virtual Node? Node { get; set; }
}