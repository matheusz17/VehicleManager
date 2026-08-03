using System.ComponentModel.DataAnnotations;
using VehicleManager.Api.Models.Enums;

namespace VehicleManager.Api.Dtos;

public class CreateVeiculoDto
{
    // DTO é o formato que a API aceita do front; ele não é a entidade salva pelo EF.
    // Aceita ABC1234 e o padrão Mercosul ABC1D23.
    [Required]
    [StringLength(8)]
    [RegularExpression(@"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$")]
    public string Placa { get; set; } = string.Empty;

    [Required]
    // Estas anotações fazem o ASP.NET devolver 400 antes de chegar ao service.
    [StringLength(50)]
    public string Marca { get; set; } = string.Empty;

    // Os enums também entram no DTO para o front enviar uma opção dos selects.
    [Required]
    [StringLength(80)]
    public string Modelo { get; set; } = string.Empty;

    // O service confere o ano corrente; este range já corta valores claramente sem sentido.
    [Range(1950, 2100)]
    public int AnoFabricacao { get; set; }

    [Range(1950, 2100)]
    public int AnoModelo { get; set; }

    [Required]
    [StringLength(30)]
    public string Cor { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Quilometragem { get; set; }

    // Não deixo preço zero nem negativo passar da validação automática da API.
    [Range(typeof(decimal), "0,01", "999999999")]
    public decimal Preco { get; set; }

    [Required]
    public Combustivel Combustivel { get; set; }

    [Required]
    public Cambio Cambio { get; set; }

    [Required]
    public StatusVeiculo Status { get; set; }

    // Sem Required, observações é realmente opcional.
    [StringLength(500)]
    public string? Observacoes { get; set; }
}
