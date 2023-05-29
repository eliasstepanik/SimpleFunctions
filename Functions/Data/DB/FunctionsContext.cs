using Microsoft.EntityFrameworkCore;

namespace Functions.Data.DB;

public class FunctionsContext : DbContext
{
    public FunctionsContext(DbContextOptions<FunctionsContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Function>()
            .HasMany(p => p.Instances)
            .WithOne(s => s.Function)
            .HasForeignKey(s => s.FunctionId);
        modelBuilder.Entity<Function>()
            .HasMany(p => p.EnvironmentVariables)
            .WithOne(s => s.Function)
            .HasForeignKey(s => s.FunctionId);
        /*modelBuilder.Entity<Product>()
            .HasMany(p => p.SellEntries)
            .WithOne(s => s.Item);

        modelBuilder.Entity<User>()
            .HasMany(u => u.SellEntries)
            .WithOne(e => e.SoldBy);
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Cart)
            .WithOne(e => e.User);*/

    }


    public DbSet<Function> Functions { get; set; } = null!;
    public DbSet<Instance> Instances { get; set; } = null!;
    public DbSet<Environment> EnvironmentVariables { get; set; } = null!;
}