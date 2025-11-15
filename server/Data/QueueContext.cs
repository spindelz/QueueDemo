using Microsoft.EntityFrameworkCore;
using server.Models;

namespace server.Data;

public class QueueContext : DbContext
{
    public QueueContext(DbContextOptions<QueueContext> options)
        : base(options)
    {
    }

    public DbSet<Queue> Queues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Queue>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Number).IsRequired();
            entity.Property(e => e.QDate);
        });
    }
}
