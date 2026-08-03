namespace VehicleManager.Api.Models;

using VehicleManager.Api.Models.Enums;
public class Veiculo
{
    // Identificador único usado nas rotas, por exemplo: /api/veiculos/{id}.
    public Guid Id { get; set; }

    // Dados principais que formam o cadastro do veículo.
    public string Placa { get; set; } = string.Empty;

    public string Marca { get; set; } = string.Empty;

    public string Modelo { get; set; } = string.Empty;

    public int AnoFabricacao { get; set; }

    public int AnoModelo { get; set; }

    public string Cor { get; set; } = string.Empty;

    public int Quilometragem { get; set; }

    public decimal Preco { get; set; }

    // Enums evitam salvar texto livre e valores diferentes para a mesma opção.
    public Combustivel Combustivel { get; set; }

    public Cambio Cambio { get; set; }

    public StatusVeiculo Status { get; set; }

    // O ? quer dizer que este campo pode ficar vazio no banco.
    public string? Observacoes { get; set; }

    // A data nasce no servidor para o cliente não conseguir alterá-la no formulário.
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
