using Microsoft.EntityFrameworkCore;
using tarea3_aplicaciones_web.Models;

namespace tarea3_aplicaciones_web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>().ToTable("clientes");
        modelBuilder.Entity<Cliente>().HasIndex(c => c.Cedula).IsUnique();
    }
}
