using Microsoft.EntityFrameworkCore;

namespace Customer.API.Persistence;

public class CustomerContext : DbContext
{
    public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
    {
    }

    public DbSet<Entities.Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Entities.Customer>().HasIndex(x => x.UserName)
            .IsUnique();
        modelBuilder.Entity<Entities.Customer>().HasIndex(x => x.EmailAddress)
            .IsUnique();
        modelBuilder.Entity<Entities.Customer>()
            .Property(x => x.UserName)
            .HasMaxLength(50);
        modelBuilder.Entity<Entities.Customer>()
            .Property(x => x.FirstName)
            .HasMaxLength(50);
        modelBuilder.Entity<Entities.Customer>()
            .Property(x => x.LastName)
            .HasMaxLength(150);
        modelBuilder.Entity<Entities.Customer>()
            .Property(x => x.EmailAddress)
            .HasMaxLength(250);
    }
}