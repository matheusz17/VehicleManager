using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManager.Api.Data;
using VehicleManager.Api.Models;
using VehicleManager.Api.Services;
using VehicleManager.Api.Dtos;

namespace VehicleManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VeiculosController : ControllerBase
{
    private readonly VeiculoService _service;

    public VeiculosController(VeiculoService service)
    {
    _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Veiculo>>> GetVeiculos()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Veiculo>> GetVeiculo(Guid id)
    {
        var veiculo = await _service.GetByIdAsync(id);

        if (veiculo == null)
            return NotFound();

        return Ok(veiculo);
    }
    [HttpPost]
    public async Task<ActionResult<VeiculoDto>> PostVeiculo(CreateVeiculoDto dto)
    {
        var novoVeiculo = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetVeiculo),
            new { id = novoVeiculo.Id },
            novoVeiculo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, UpdateVeiculoDto dto)
    {
        var atualizado = await _service.UpdateAsync(id, dto);

        if (!atualizado)
            return NotFound();

        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var removido = await _service.DeleteAsync(id);

        if (!removido)
            return NotFound();

        return NoContent();
    }
}