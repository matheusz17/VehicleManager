using Microsoft.EntityFrameworkCore;
using VehicleManager.Api.Models;

namespace VehicleManager.Api.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Veiculo> Veiculos { get; set; }
}