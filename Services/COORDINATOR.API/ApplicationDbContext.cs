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
                new Node() { Id = new Guid("9f41f2eb-0854-435b-800f-72c95b45ede7"), Name = "Order.API" },
                new Node() { Id = new Guid("2b4496c4-ad81-445b-9375-a63662d82a7b"), Name = "Stock.API" },
                new Node() { Id = new Guid("b447fe00-7cd8-444f-8142-60ee8397eb3a"), Name = "Payment.API" }
            );
    }
}
