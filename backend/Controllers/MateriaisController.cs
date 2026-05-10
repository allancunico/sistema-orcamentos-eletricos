using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrcamentosAPI.Data;
using OrcamentosAPI.DTOs;
using OrcamentosAPI.Models;

namespace OrcamentosAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MateriaisController : ControllerBase
{
    private readonly AppDbContext _db;
    public MateriaisController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool apenasAtivos = true)
    {
        var query = _db.Materiais.AsQueryable();
        if (apenasAtivos) query = query.Where(m => m.Ativo);

        var lista = await query
            .OrderBy(m => m.Nome)
            .Select(m => new MaterialDto(m.MaterialId, m.Nome, m.Unidade, m.PrecoUnitario, m.Ativo))
            .ToListAsync();

        return Ok(lista);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var m = await _db.Materiais.FindAsync(id);
        if (m == null) return NotFound();
        return Ok(new MaterialDto(m.MaterialId, m.Nome, m.Unidade, m.PrecoUnitario, m.Ativo));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMaterialDto dto)
    {
        var material = new Material
        {
            Nome = dto.Nome,
            Unidade = dto.Unidade,
            PrecoUnitario = dto.PrecoUnitario
        };
        _db.Materiais.Add(material);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = material.MaterialId },
            new MaterialDto(material.MaterialId, material.Nome, material.Unidade, material.PrecoUnitario, material.Ativo));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateMaterialDto dto)
    {
        var material = await _db.Materiais.FindAsync(id);
        if (material == null) return NotFound();

        material.Nome = dto.Nome;
        material.Unidade = dto.Unidade;
        material.PrecoUnitario = dto.PrecoUnitario;
        material.Ativo = dto.Ativo;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
