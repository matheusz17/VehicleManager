using VehicleManager.Api.Models.Enums;

namespace VehicleManager.Api.Dtos;

public class VeiculoDto
{
    // Este é o formato que sai da API e que o Vue usa para preencher lista e edição.
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
    // Vai na resposta para consulta, mas nunca vem nos DTOs de criação/edição.
    public DateTime CriadoEm { get; set; }
}
