using Microsoft.EntityFrameworkCore;
using VehicleManager.Api.Data;
using VehicleManager.Api.Models;
using VehicleManager.Api.Dtos;
using VehicleManager.Api.Mappers;

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
        var veiculo = dto.ToEntity();

        _context.Veiculos.Add(veiculo);

        await _context.SaveChangesAsync();

        return veiculo.ToDto();
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateVeiculoDto dto)
    {
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
}