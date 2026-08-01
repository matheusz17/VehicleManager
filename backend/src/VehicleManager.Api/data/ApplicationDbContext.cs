using Microsoft.EntityFrameworkCore;

namespace VehicleManager.Api.data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}