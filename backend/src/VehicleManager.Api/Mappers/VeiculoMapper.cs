using VehicleManager.Api.Dtos;
using VehicleManager.Api.Models;

namespace VehicleManager.Api.Mappers;

public static class VeiculoMapper
{
    public static VeiculoDto ToDto(this Veiculo veiculo)
    {
        return new VeiculoDto
        {
            Id = veiculo.Id,
            Placa = veiculo.Placa,
            Marca = veiculo.Marca,
            Modelo = veiculo.Modelo,
            AnoFabricacao = veiculo.AnoFabricacao,
            AnoModelo = veiculo.AnoModelo,
            Cor = veiculo.Cor,
            Quilometragem = veiculo.Quilometragem,
            Preco = veiculo.Preco,
            Combustivel = veiculo.Combustivel,
            Cambio = veiculo.Cambio,
            Status = veiculo.Status,
            Observacoes = veiculo.Observacoes,
            CriadoEm = veiculo.CriadoEm
        };
    }

    public static Veiculo ToEntity(this CreateVeiculoDto dto)
    {
        return new Veiculo
        {
            Id = Guid.NewGuid(),
            Placa = dto.Placa,
            Marca = dto.Marca,
            Modelo = dto.Modelo,
            AnoFabricacao = dto.AnoFabricacao,
            AnoModelo = dto.AnoModelo,
            Cor = dto.Cor,
            Quilometragem = dto.Quilometragem,
            Preco = dto.Preco,
            Combustivel = dto.Combustivel,
            Cambio = dto.Cambio,
            Status = dto.Status,
            Observacoes = dto.Observacoes,
            CriadoEm = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this Veiculo veiculo, UpdateVeiculoDto dto)
    {
        veiculo.Placa = dto.Placa;
        veiculo.Marca = dto.Marca;
        veiculo.Modelo = dto.Modelo;
        veiculo.AnoFabricacao = dto.AnoFabricacao;
        veiculo.AnoModelo = dto.AnoModelo;
        veiculo.Cor = dto.Cor;
        veiculo.Quilometragem = dto.Quilometragem;
        veiculo.Preco = dto.Preco;
        veiculo.Combustivel = dto.Combustivel;
        veiculo.Cambio = dto.Cambio;
        veiculo.Status = dto.Status;
        veiculo.Observacoes = dto.Observacoes;
    }
}