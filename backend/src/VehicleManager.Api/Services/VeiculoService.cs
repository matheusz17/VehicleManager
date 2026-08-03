using Microsoft.EntityFrameworkCore;
using VehicleManager.Api.Data;
using VehicleManager.Api.Models;
using VehicleManager.Api.Dtos;
using VehicleManager.Api.Mappers;
using VehicleManager.Api.Models.Enums;

namespace VehicleManager.Api.Services;

public class VeiculoService
{
    // O DbContext é a camada que conversa com o EF/PostgreSQL neste projeto simples.
    private readonly ApplicationDbContext _context;

    public VeiculoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VeiculoDto>> GetAllAsync(string? busca)
    {
        // Começo com a consulta inteira e só acrescento filtro se a pessoa pesquisou algo.
        var query = _context.Veiculos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            // Tirar espaços evita uma busca que parece preenchida, mas não encontra nada útil.
            busca = busca.Trim();

            // Uma única caixa de busca cobre marca, modelo e placa, como pede o desafio.
            query = query.Where(v =>
                v.Marca.Contains(busca) ||
                v.Modelo.Contains(busca) ||
                v.Placa.Contains(busca));
        }

        // Só executo a consulta aqui; até este ponto ela ainda está sendo montada pelo EF.
        var veiculos = await query.ToListAsync();

        // A entidade nunca sai da API diretamente: sempre converto para o DTO de resposta.
        return veiculos.Select(VeiculoMapper.ToDto);
    }

    public async Task<VeiculoDto?> GetByIdAsync(Guid id)
    {
        // FindAsync é direto para buscar pela chave primária.
        var veiculo = await _context.Veiculos.FindAsync(id);

        return veiculo?.ToDto();
    }

    public async Task<VeiculoDto> CreateAsync(CreateVeiculoDto dto)
    {
        // Data Annotations cuidam das regras simples; aqui ficam as regras que dependem de comparação.
        ValidarVeiculo(dto);

        // Confiro antes de salvar para conseguir retornar 409 em vez de deixar o banco explodir a exceção.
        var placaExiste = await _context.Veiculos
            .AnyAsync(v => v.Placa == dto.Placa);

        if (placaExiste)
            throw new InvalidOperationException("Já existe um veículo com esta placa.");

        // O mapper evita copiar campo por campo dentro do fluxo de criação.
        var veiculo = VeiculoMapper.ToEntity(dto);

        // A data é responsabilidade da API; o cliente não consegue mandar esse valor.
        veiculo.CriadoEm = DateTime.UtcNow;

        _context.Veiculos.Add(veiculo);
        await _context.SaveChangesAsync();

        return VeiculoMapper.ToDto(veiculo);
    }
    

    public async Task<bool> UpdateAsync(Guid id, UpdateVeiculoDto dto)
    {
        ValidarVeiculo(dto);

        // No update ignoro o próprio veículo, senão ele sempre acusaria a própria placa como duplicada.
        var placaExiste = await _context.Veiculos
            .AnyAsync(v => v.Placa == dto.Placa && v.Id != id);

        if (placaExiste)
            throw new InvalidOperationException("Já existe um veículo com esta placa.");

        var veiculo = await _context.Veiculos.FindAsync(id);

        if (veiculo == null)
            return false;

        // Atualizo somente os campos que podem mudar; ID e data de criação ficam intactos.
        veiculo.UpdateEntity(dto);

        await _context.SaveChangesAsync();

        return true;
    }    
    public async Task<bool> DeleteAsync(Guid id)
    {
        // Primeiro confiro se existe para devolver 404 de forma previsível.
        var existente = await _context.Veiculos.FindAsync(id);

        if (existente is null)
            return false;

        _context.Veiculos.Remove(existente);
        await _context.SaveChangesAsync();

        return true;
    }

    private static void ValidarVeiculo(CreateVeiculoDto dto)
    {
        // O limite acompanha o calendário, então não deixo um número fixo no código.
        var anoMaximo = DateTime.UtcNow.Year + 1;

        if (dto.AnoFabricacao < 1950 || dto.AnoFabricacao > anoMaximo)
            throw new ArgumentException("Ano de fabricação inválido.");

        // O modelo não pode ser anterior à fabricação.
        if (dto.AnoModelo < dto.AnoFabricacao || dto.AnoModelo > anoMaximo)
            throw new ArgumentException("Ano do modelo inválido.");

        // Mesmo que alguém burle o select do front, a API ainda valida os enums.
        if (!Enum.IsDefined(typeof(Combustivel), dto.Combustivel))
            throw new ArgumentException("Combustível inválido.");

        if (!Enum.IsDefined(typeof(Cambio), dto.Cambio))
            throw new ArgumentException("Câmbio inválido.");

        if (!Enum.IsDefined(typeof(StatusVeiculo), dto.Status))
            throw new ArgumentException("Status inválido.");
    }

    private static void ValidarVeiculo(UpdateVeiculoDto dto)
    {
        // Repito essas regras no update para ninguém salvar uma edição inválida pela API.
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
