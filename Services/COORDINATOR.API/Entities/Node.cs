using System.ComponentModel.DataAnnotations;

namespace COORDINATOR.API.Entities;

public class Node
{
    [Key]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public virtual ICollection<NodeState>? NodeStates { get; set; }
}


