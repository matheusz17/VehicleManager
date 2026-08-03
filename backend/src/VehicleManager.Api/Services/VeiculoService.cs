using Microsoft.EntityFrameworkCore;
using VehicleManager.Api.Data;
using VehicleManager.Api.Models;
using VehicleManager.Api.Dtos;
using VehicleManager.Api.Mappers;
using VehicleManager.Api.Models.Enums;

namespace VehicleManager.Api.Services;

public class VeiculoService
{
    private readonly ApplicationDbContext _context;

    public VeiculoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<VeiculoDto>> GetAllAsync()
    {
        var veiculos = await _context.Veiculos.ToListAsync();

        return veiculos.Select(v => v.ToDto()).ToList();
    }

    public async Task<VeiculoDto?> GetByIdAsync(Guid id)
    {
        var veiculo = await _context.Veiculos.FindAsync(id);

        return veiculo?.ToDto();
    }

    public async Task<VeiculoDto> CreateAsync(CreateVeiculoDto dto)
    {
        ValidarVeiculo(dto);

        var placaExiste = await _context.Veiculos
            .AnyAsync(v => v.Placa == dto.Placa);

        if (placaExiste)
            throw new InvalidOperationException("Já existe um veículo com esta placa.");

        var veiculo = VeiculoMapper.ToEntity(dto);

        veiculo.CriadoEm = DateTime.UtcNow;

        _context.Veiculos.Add(veiculo);
        await _context.SaveChangesAsync();

        return VeiculoMapper.ToDto(veiculo);
    }
    

    public async Task<bool> UpdateAsync(Guid id, UpdateVeiculoDto dto)
    {
        ValidarVeiculo(dto);

        var placaExiste = await _context.Veiculos
            .AnyAsync(v => v.Placa == dto.Placa && v.Id != id);

        if (placaExiste)
            throw new InvalidOperationException("Já existe um veículo com esta placa.");

        var veiculo = await _context.Veiculos.FindAsync(id);

        if (veiculo == null)
            return false;

        veiculo.UpdateEntity(dto);

        await _context.SaveChangesAsync();

        return true;
    }    
    public async Task<bool> DeleteAsync(Guid id)
    {
        var existente = await _context.Veiculos.FindAsync(id);

        if (existente is null)
            return false;

        _context.Veiculos.Remove(existente);
        await _context.SaveChangesAsync();

        return true;
    }

    private static void ValidarVeiculo(CreateVeiculoDto dto)
{
    var anoMaximo = DateTime.UtcNow.Year + 1;

    if (dto.AnoFabricacao < 1950 || dto.AnoFabricacao > anoMaximo)
        throw new ArgumentException("Ano de fabricação inválido.");

    if (dto.AnoModelo < dto.AnoFabricacao || dto.AnoModelo > anoMaximo)
        throw new ArgumentException("Ano do modelo inválido.");

    if (!Enum.IsDefined(typeof(Combustivel), dto.Combustivel))
        throw new ArgumentException("Combustível inválido.");

    if (!Enum.IsDefined(typeof(Cambio), dto.Cambio))
        throw new ArgumentException("Câmbio inválido.");

    if (!Enum.IsDefined(typeof(StatusVeiculo), dto.Status))
        throw new ArgumentException("Status inválido.");
}

    private static void ValidarVeiculo(UpdateVeiculoDto dto)
    {
        var anoMaximo = DateTime.UtcNow.Year + 1;

    if (dto.AnoFabricacao < 1950 || dto.AnoFabricacao > anoMaximo)
        throw new ArgumentException("Ano de fabricação inválido.");

    if (dto.AnoModelo < dto.AnoFabricacao || dto.AnoModelo > anoMaximo)
        throw new ArgumentException("Ano do modelo inválido.");

    if (!Enum.IsDefined(typeof(Combustivel), dto.Combustivel))
        throw new ArgumentException("Combustível inválido.");

    if (!Enum.IsDefined(typeof(Cambio), dto.Cambio))
        throw new ArgumentException("Câmbio inválido.");

    if (!Enum.IsDefined(typeof(StatusVeiculo), dto.Status))
        throw new ArgumentException("Status inválido.");
    }
}