using Microsoft.EntityFrameworkCore;
using VehicleManager.Api.Models;

namespace VehicleManager.Api.Data;
public class ApplicationDbContext : DbContext
{
    // O EF entrega as opções prontas (inclusive a conexão configurada no Program) por injeção.
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Representa a tabela Veiculos e é por aqui que o EF monta as consultas.
    public DbSet<Veiculo> Veiculos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Primeiro mantenho qualquer configuração padrão que o EF já teria aplicado.
        base.OnModelCreating(modelBuilder);

        // A validação na API melhora a mensagem, e este índice protege a regra no banco também.
        modelBuilder.Entity<Veiculo>()
            .HasIndex(veiculo => veiculo.Placa)
            .IsUnique();

        modelBuilder.Entity<Veiculo>(entity =>
        {
            // Espelho no banco os limites que também existem nos DTOs.
            entity.Property(veiculo => veiculo.Placa).HasMaxLength(8);
            entity.Property(veiculo => veiculo.Marca).HasMaxLength(50);
            entity.Property(veiculo => veiculo.Modelo).HasMaxLength(80);
            entity.Property(veiculo => veiculo.Cor).HasMaxLength(30);
            entity.Property(veiculo => veiculo.Observacoes).HasMaxLength(500);
            // Preço com duas casas para não guardar uma precisão inesperada.
            entity.Property(veiculo => veiculo.Preco).HasPrecision(10, 2);
        });
        // Quando este modelo muda, gero uma migration para levar a alteração ao PostgreSQL.
    }
}
