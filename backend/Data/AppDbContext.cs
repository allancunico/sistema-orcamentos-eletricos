using Microsoft.EntityFrameworkCore;
using OrcamentosAPI.Models;

namespace OrcamentosAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Material> Materiais => Set<Material>();
    public DbSet<Tecnico> Tecnicos => Set<Tecnico>();
    public DbSet<Orcamento> Orcamentos => Set<Orcamento>();
    public DbSet<ItemOrcamento> ItensOrcamento => Set<ItemOrcamento>();
    public DbSet<MaoDeObra> MaoDeObra => Set<MaoDeObra>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemOrcamento>().HasKey(i => i.ItemId);
        modelBuilder.Entity<MaoDeObra>().HasKey(m => m.MaoDeObraId);
        modelBuilder.Entity<Orcamento>().HasKey(o => o.OrcamentoId);
        modelBuilder.Entity<Material>().HasKey(m => m.MaterialId);
        modelBuilder.Entity<Tecnico>().HasKey(t => t.TecnicoId);

        modelBuilder.Entity<Material>().Property(m => m.PrecoUnitario).HasPrecision(10, 2);
        modelBuilder.Entity<Tecnico>().Property(t => t.ValorHora).HasPrecision(10, 2);
        modelBuilder.Entity<ItemOrcamento>().Property(i => i.PrecoUnitario).HasPrecision(10, 2);
        modelBuilder.Entity<ItemOrcamento>().Property(i => i.Quantidade).HasPrecision(10, 3);
        modelBuilder.Entity<MaoDeObra>().Property(m => m.ValorHora).HasPrecision(10, 2);
        modelBuilder.Entity<MaoDeObra>().Property(m => m.Horas).HasPrecision(10, 2);

        modelBuilder.Entity<Orcamento>().Ignore(o => o.TotalMateriais);
        modelBuilder.Entity<Orcamento>().Ignore(o => o.TotalMaoDeObra);
        modelBuilder.Entity<Orcamento>().Ignore(o => o.TotalGeral);
        modelBuilder.Entity<Orcamento>().Ignore(o => o.HorasEstimadas);
        modelBuilder.Entity<ItemOrcamento>().Ignore(i => i.Subtotal);
        modelBuilder.Entity<MaoDeObra>().Ignore(m => m.Subtotal);

        var d = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Tecnico>().HasData(
            new Tecnico { TecnicoId = 1, Nome = "Carlos Silva", Cargo = "Tecnico Senior", ValorHora = 120.00m, Ativo = true },
            new Tecnico { TecnicoId = 2, Nome = "Ana Souza", Cargo = "Tecnico Junior", ValorHora = 80.00m, Ativo = true },
            new Tecnico { TecnicoId = 3, Nome = "Roberto Lima", Cargo = "Engenheiro", ValorHora = 200.00m, Ativo = true }
        );

        modelBuilder.Entity<Material>().HasData(
            new Material { MaterialId = 1, Nome = "Disjuntor Monofasico 10A", Unidade = "un", PrecoUnitario = 18.50m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 2, Nome = "Disjuntor Monofasico 20A", Unidade = "un", PrecoUnitario = 19.80m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 3, Nome = "Disjuntor Monofasico 30A", Unidade = "un", PrecoUnitario = 21.00m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 4, Nome = "Disjuntor Bifasico 20A", Unidade = "un", PrecoUnitario = 45.00m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 5, Nome = "Disjuntor Trifasico 40A", Unidade = "un", PrecoUnitario = 89.00m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 6, Nome = "Disjuntor DR 25A/30mA", Unidade = "un", PrecoUnitario = 95.00m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 7, Nome = "Barramento de cobre 100A", Unidade = "un", PrecoUnitario = 35.00m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 8, Nome = "Fio 2,5mm", Unidade = "m", PrecoUnitario = 3.80m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 9, Nome = "Fio 4mm", Unidade = "m", PrecoUnitario = 5.90m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 10, Nome = "Fio 6mm", Unidade = "m", PrecoUnitario = 8.50m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 11, Nome = "Fio 10mm", Unidade = "m", PrecoUnitario = 14.00m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 12, Nome = "Cabo PP 3x2,5mm", Unidade = "m", PrecoUnitario = 12.00m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 13, Nome = "Quadro distribuicao 12 disjuntores", Unidade = "un", PrecoUnitario = 85.00m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 14, Nome = "Quadro distribuicao 24 disjuntores", Unidade = "un", PrecoUnitario = 145.00m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 15, Nome = "Tomada 2P+T 20A", Unidade = "un", PrecoUnitario = 12.00m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 16, Nome = "Tomada 2P+T 10A", Unidade = "un", PrecoUnitario = 8.50m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 17, Nome = "Interruptor simples", Unidade = "un", PrecoUnitario = 7.00m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 18, Nome = "Eletroduto 3/4", Unidade = "m", PrecoUnitario = 4.20m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 19, Nome = "Abracadeira plastica pct 100", Unidade = "pct", PrecoUnitario = 9.00m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 20, Nome = "Terminal ilhos 2,5mm", Unidade = "un", PrecoUnitario = 1.20m, Ativo = true, CriadoEm = d },
            new Material { MaterialId = 21, Nome = "Fita isolante 18m", Unidade = "un", PrecoUnitario = 5.50m, Ativo = true, CriadoEm = d }
        );
    }
}
