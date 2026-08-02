namespace VehicleManager.Api.Models;

using VehicleManager.Api.Models.Enums;
public class Veiculo
{
    public Guid Id { get; set; }

    public string Placa { get; set; } = string.Empty;

    public string Marca { get; set; } = string.Empty;

    public string Modelo { get; set; } = string.Empty;

    public int AnoFabricacao { get; set; }

    public int AnoModelo { get; set; }

    public string Cor { get; set; } = string.Empty;

    public int Quilometragem { get; set; }

    public decimal Preco { get; set; }

    public Combustivel Combustivel { get; set; }

    public Cambio Cambio { get; set; }

    public StatusVeiculo Status { get; set; }

    public string? Observacoes { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}