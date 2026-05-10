using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrcamentosAPI.Data;
using OrcamentosAPI.DTOs;
using OrcamentosAPI.Models;
using OrcamentosAPI.Services;


namespace OrcamentosAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrcamentosController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ExcelExportService _excel;

    public OrcamentosController(AppDbContext db, ExcelExportService excel)
    {
        _db = db;
        _excel = excel;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] string? cliente,
        [FromQuery] DateTime? de,
        [FromQuery] DateTime? ate)
    {
        var query = _db.Orcamentos
            .Include(o => o.Itens)
            .Include(o => o.MaoDeObra)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(o => o.Status == status);
        if (!string.IsNullOrEmpty(cliente))
            query = query.Where(o => o.NomeCliente.Contains(cliente));
        if (de.HasValue)
            query = query.Where(o => o.CriadoEm >= de.Value);
        if (ate.HasValue)
            query = query.Where(o => o.CriadoEm <= ate.Value.AddDays(1));

        var lista = await query
            .OrderByDescending(o => o.CriadoEm)
            .ToListAsync();

        return Ok(lista.Select(ToResumo));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var orc = await _db.Orcamentos
            .Include(o => o.Itens)
            .Include(o => o.MaoDeObra)
            .FirstOrDefaultAsync(o => o.OrcamentoId == id);

        if (orc == null) return NotFound();
        return Ok(ToDetalhe(orc));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrcamentoDto dto)
    {
        var orc = new Orcamento
        {
            NomeCliente = dto.NomeCliente,
            DescricaoServico = dto.DescricaoServico
        };

        await AdicionarItens(orc, dto.Itens);
        await AdicionarMaoDeObra(orc, dto.MaoDeObra);

        _db.Orcamentos.Add(orc);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = orc.OrcamentoId }, ToResumo(orc));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateOrcamentoDto dto)
    {
        var orc = await _db.Orcamentos
            .Include(o => o.Itens)
            .Include(o => o.MaoDeObra)
            .FirstOrDefaultAsync(o => o.OrcamentoId == id);

        if (orc == null) return NotFound();

        orc.NomeCliente = dto.NomeCliente;
        orc.DescricaoServico = dto.DescricaoServico;
        orc.Status = dto.Status;
        orc.AtualizadoEm = DateTime.UtcNow;

        // Recria itens e mão de obra
        _db.ItensOrcamento.RemoveRange(orc.Itens);
        _db.MaoDeObra.RemoveRange(orc.MaoDeObra);
        orc.Itens.Clear();
        orc.MaoDeObra.Clear();

        await AdicionarItens(orc, dto.Itens);
        await AdicionarMaoDeObra(orc, dto.MaoDeObra);

        await _db.SaveChangesAsync();
        return Ok(ToDetalhe(orc));
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> AtualizarStatus(int id, [FromBody] string novoStatus)
    {
        var statusValidos = new[] { "Rascunho", "Enviado", "Aprovado", "Cancelado" };
        if (!statusValidos.Contains(novoStatus))
            return BadRequest("Status inválido.");

        var orc = await _db.Orcamentos.FindAsync(id);
        if (orc == null) return NotFound();

        orc.Status = novoStatus;
        orc.AtualizadoEm = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}/exportar")]
    public async Task<IActionResult> Exportar(int id)
    {
        var orc = await _db.Orcamentos
            .Include(o => o.Itens)
            .Include(o => o.MaoDeObra)
            .FirstOrDefaultAsync(o => o.OrcamentoId == id);

        if (orc == null) return NotFound();

        var bytes = _excel.Gerar(orc);
        var nomeArquivo = $"Orcamento_{orc.OrcamentoId:D4}_{orc.NomeCliente.Replace(" ", "_")}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nomeArquivo);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────
    private async Task AdicionarItens(Orcamento orc, List<CreateItemDto> itens)
    {
        foreach (var i in itens)
        {
            var mat = await _db.Materiais.FindAsync(i.MaterialId)
                ?? throw new Exception($"Material {i.MaterialId} não encontrado.");
            orc.Itens.Add(new ItemOrcamento
            {
                MaterialId = mat.MaterialId,
                NomeMaterial = mat.Nome,
                Unidade = mat.Unidade,
                PrecoUnitario = mat.PrecoUnitario,
                Quantidade = i.Quantidade
            });
        }
    }

    private async Task AdicionarMaoDeObra(Orcamento orc, List<CreateMaoDeObraDto> lista)
    {
        foreach (var m in lista)
        {
            var tec = await _db.Tecnicos.FindAsync(m.TecnicoId)
                ?? throw new Exception($"Técnico {m.TecnicoId} não encontrado.");
            orc.MaoDeObra.Add(new MaoDeObra
            {
                TecnicoId = tec.TecnicoId,
                NomeTecnico = tec.Nome,
                Cargo = tec.Cargo,
                ValorHora = tec.ValorHora,
                Horas = m.Horas,
                NumeroTecnicos = m.NumeroTecnicos
            });
        }
    }

    private static OrcamentoResumoDto ToResumo(Orcamento o) => new(
        o.OrcamentoId, o.NomeCliente, o.DescricaoServico, o.Status,
        o.CriadoEm, o.AtualizadoEm,
        o.TotalMateriais, o.TotalMaoDeObra, o.TotalGeral, o.HorasEstimadas
    );

    private static OrcamentoDetalheDto ToDetalhe(Orcamento o) => new(
        o.OrcamentoId, o.NomeCliente, o.DescricaoServico, o.Status,
        o.CriadoEm, o.AtualizadoEm,
        o.Itens.Select(i => new ItemOrcamentoDto(
            i.ItemId, i.MaterialId, i.NomeMaterial, i.Unidade,
            i.PrecoUnitario, i.Quantidade, i.Subtotal)).ToList(),
        o.MaoDeObra.Select(m => new MaoDeObraDto(
            m.MaoDeObraId, m.TecnicoId, m.NomeTecnico, m.Cargo,
            m.ValorHora, m.Horas, m.NumeroTecnicos, m.Subtotal)).ToList(),
        o.TotalMateriais, o.TotalMaoDeObra, o.TotalGeral, o.HorasEstimadas
    );
}
