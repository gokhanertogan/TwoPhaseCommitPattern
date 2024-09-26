using COORDINATOR.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace COORDINATOR.API;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Node> Nodes { get; set; }
    public DbSet<NodeState> NodeStates { get; set; }
    public DbSet<TransactionData> TransactionDatas { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NodeState>()
            .HasOne(ns => ns.Node)
            .WithMany(n => n.NodeStates)
            .HasForeignKey(ns => ns.NodeId);

        modelBuilder.Entity<Node>().HasData(
                new Node() { Id = Guid.NewGuid(), Name = "Order.API" },
                new Node() { Id = Guid.NewGuid(), Name = "Stock.API" },
                new Node() { Id = Guid.NewGuid(), Name = "Payment.API" }
            );
    }
}
