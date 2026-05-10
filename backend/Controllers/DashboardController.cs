using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrcamentosAPI.Data;
using OrcamentosAPI.DTOs;

namespace OrcamentosAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int ano = 0)
    {
        if (ano == 0) ano = DateTime.UtcNow.Year;

        var orcamentos = await _db.Orcamentos
            .Include(o => o.Itens)
            .Include(o => o.MaoDeObra)
            .Where(o => o.CriadoEm.Year == ano)
            .ToListAsync();

        var porMes = Enumerable.Range(1, 12).Select(mes =>
        {
            var doMes = orcamentos.Where(o => o.CriadoEm.Month == mes).ToList();
            return new OrcamentosPorMesDto(
                new DateTime(ano, mes, 1).ToString("MMM"),
                doMes.Count,
                doMes.Sum(o => o.TotalGeral)
            );
        }).ToList();

        // Busca itens em memória para evitar erro de tradução LINQ
        var todosItens = await _db.ItensOrcamento
            .Include(i => i.Orcamento)
            .Where(i => i.Orcamento!.CriadoEm.Year == ano)
            .ToListAsync();

        var materiaisMaisUsados = todosItens
            .GroupBy(i => i.NomeMaterial)
            .Select(g => new MaterialMaisUsadoDto(g.Key, g.Sum(i => i.Quantidade)))
            .OrderByDescending(m => m.QuantidadeTotal)
            .Take(5)
            .ToList();

        var dashboard = new DashboardDto(
            TotalOrcamentos: orcamentos.Count,
            ValorTotalOrcado: orcamentos.Sum(o => o.TotalGeral),
            ValorTotalAprovado: orcamentos.Where(o => o.Status == "Aprovado").Sum(o => o.TotalGeral),
            OrcamentosAprovados: orcamentos.Count(o => o.Status == "Aprovado"),
            OrcamentosEnviados: orcamentos.Count(o => o.Status == "Enviado"),
            OrcamentosRascunho: orcamentos.Count(o => o.Status == "Rascunho"),
            OrcamentosCancelados: orcamentos.Count(o => o.Status == "Cancelado"),
            PorMes: porMes,
            MateriaisMaisUsados: materiaisMaisUsados
        );

        return Ok(dashboard);
    }
}