using System.ComponentModel.DataAnnotations;
using VehicleManager.Api.Models.Enums;

namespace VehicleManager.Api.Dtos;

public class CreateVeiculoDto
{
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

    [StringLength(500)]
    public string? Observacoes { get; set; }
}