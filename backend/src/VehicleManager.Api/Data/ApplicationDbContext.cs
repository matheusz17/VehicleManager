namespace VehicleManager.Api.Data;

using Microsoft.EntityFrameworkCore;
using VehicleManager.Api.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Veiculo> Veiculos { get; set; }
}