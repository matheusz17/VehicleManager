using System.ComponentModel.DataAnnotations;
using VehicleManager.Api.Models.Enums;

namespace VehicleManager.Api.Dtos;

public class UpdateVeiculoDto
{
    // É separado do DTO de criação para deixar claro o contrato de cada operação.
    // Hoje os campos editáveis são iguais, mas Id e CriadoEm continuam fora daqui.
    [Required]
    [StringLength(8)]
    [RegularExpression(@"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$")]
    public string Placa { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Marca { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Modelo { get; set; } = string.Empty;

    // O service ainda compara os dois anos e aplica o limite do ano atual.
    [Range(1950, 2100)]
    public int AnoFabricacao { get; set; }

    [Range(1950, 2100)]
    public int AnoModelo { get; set; }

    [Required]
    [StringLength(30)]
    public string Cor { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Quilometragem { get; set; }

    [Range(typeof(decimal), "0,01", "999999999")]
    public decimal Preco { get; set; }

    [Required]
    public Combustivel Combustivel { get; set; }

    [Required]
    public Cambio Cambio { get; set; }

    [Required]
    public StatusVeiculo Status { get; set; }

    // Campo livre, porém limitado para não crescer sem controle no banco.
    [StringLength(500)]
    public string? Observacoes { get; set; }
}
