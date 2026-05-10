using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrcamentosAPI.Data;
using OrcamentosAPI.DTOs;
using OrcamentosAPI.Models;

namespace OrcamentosAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TecnicosController : ControllerBase
{
    private readonly AppDbContext _db;
    public TecnicosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool apenasAtivos = true)
    {
        var query = _db.Tecnicos.AsQueryable();
        if (apenasAtivos) query = query.Where(t => t.Ativo);

        var lista = await query
            .OrderBy(t => t.Nome)
            .Select(t => new TecnicoDto(t.TecnicoId, t.Nome, t.Cargo, t.ValorHora, t.Ativo))
            .ToListAsync();

        return Ok(lista);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTecnicoDto dto)
    {
        var tecnico = new Tecnico { Nome = dto.Nome, Cargo = dto.Cargo, ValorHora = dto.ValorHora };
        _db.Tecnicos.Add(tecnico);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = tecnico.TecnicoId },
            new TecnicoDto(tecnico.TecnicoId, tecnico.Nome, tecnico.Cargo, tecnico.ValorHora, tecnico.Ativo));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateTecnicoDto dto)
    {
        var tecnico = await _db.Tecnicos.FindAsync(id);
        if (tecnico == null) return NotFound();

        tecnico.Nome = dto.Nome;
        tecnico.Cargo = dto.Cargo;
        tecnico.ValorHora = dto.ValorHora;
        tecnico.Ativo = dto.Ativo;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
