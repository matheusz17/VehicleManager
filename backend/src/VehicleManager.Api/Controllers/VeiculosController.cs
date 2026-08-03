using Microsoft.AspNetCore.Mvc;
using VehicleManager.Api.Services;
using VehicleManager.Api.Dtos;

namespace VehicleManager.Api.Controllers;

[ApiController]
// [ApiController] também ativa a resposta automática 400 para Data Annotations inválidas.
[Route("api/[controller]")]
public class VeiculosController : ControllerBase
{
    // O controller só orquestra HTTP; toda regra e acesso ao banco passam pelo service.
    private readonly VeiculoService _service;

    public VeiculosController(VeiculoService service)
    {
        _service = service;
    }

    // A busca é opcional e o service decide em quais campos procurar.
    [HttpGet]
    // ActionResult deixa o método devolver tanto 200 quanto outros status HTTP, se precisasse.
    public async Task<ActionResult<IEnumerable<VeiculoDto>>> GetVeiculos(
        [FromQuery] string? busca)
    {
        return Ok(await _service.GetAllAsync(busca));
    }

    // O constraint guid já bloqueia IDs que não têm o formato esperado.
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VeiculoDto>> GetVeiculo(Guid id)
    {
        var veiculo = await _service.GetByIdAsync(id);

        if (veiculo == null)
            // Não achar o registro é diferente de uma lista vazia: aqui devolvo 404.
            return NotFound();

        return Ok(veiculo);
    }
    [HttpPost]
    // O ASP.NET transforma automaticamente o JSON do corpo em CreateVeiculoDto.
    public async Task<ActionResult<VeiculoDto>> PostVeiculo(CreateVeiculoDto dto)
    {
        try
        {
            var veiculo = await _service.CreateAsync(dto);

            // CreatedAtAction devolve 201 e já informa a URL do veículo recém-criado.
            return CreatedAtAction(
                nameof(GetVeiculo),
                new { id = veiculo.Id },
                veiculo);
        }
        catch (ArgumentException ex)
        {
            // Regras de negócio inválidas viram 400, não erro interno.
            return BadRequest(ex.Message);
        }

        catch (InvalidOperationException ex)
        {
            // Uso 409 especificamente quando a placa já está em uso.
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    // O id vem da URL e os dados novos vêm do JSON do corpo da requisição.
    public async Task<IActionResult> Put(Guid id, UpdateVeiculoDto dto)
    {
        try
        {
            var atualizado = await _service.UpdateAsync(id, dto);

            if (!atualizado)
                return NotFound();

            // A atualização deu certo e não preciso repetir o objeto na resposta.
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // Aqui o id também é Guid para ficar consistente com as outras rotas por identificador.
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var removido = await _service.DeleteAsync(id);

        if (!removido)
            return NotFound();

        // Exclusão bem-sucedida não precisa de corpo na resposta.
        return NoContent();
    }
}
