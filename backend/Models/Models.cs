namespace OrcamentosAPI.Models;

public class Material
{
    public int MaterialId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Unidade { get; set; } = string.Empty; // "un", "m", "pct"
    public decimal PrecoUnitario { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}

public class Tecnico
{
    public int TecnicoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty; // "Técnico Sênior", "Técnico Júnior", "Engenheiro"
    public decimal ValorHora { get; set; }
    public bool Ativo { get; set; } = true;
}

public class Orcamento
{
    public int OrcamentoId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string DescricaoServico { get; set; } = string.Empty;
    public string Status { get; set; } = "Rascunho"; // Rascunho | Enviado | Aprovado | Cancelado
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<ItemOrcamento> Itens { get; set; } = new List<ItemOrcamento>();
    public ICollection<MaoDeObra> MaoDeObra { get; set; } = new List<MaoDeObra>();

    // Calculados (não persistidos)
    public decimal TotalMateriais => Itens.Sum(i => i.Subtotal);
    public decimal TotalMaoDeObra => MaoDeObra.Sum(m => m.Subtotal);
    public decimal TotalGeral => TotalMateriais + TotalMaoDeObra;
    public decimal HorasEstimadas => MaoDeObra.Sum(m => m.Horas);
}

public class ItemOrcamento
{
    public int ItemId { get; set; }
    public int OrcamentoId { get; set; }
    public int MaterialId { get; set; }

    // Copiados no momento da inserção (preserva histórico)
    public string NomeMaterial { get; set; } = string.Empty;
    public string Unidade { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public decimal Quantidade { get; set; }

    public decimal Subtotal => Quantidade * PrecoUnitario;

    public Orcamento? Orcamento { get; set; }
    public Material? Material { get; set; }
}

public class MaoDeObra
{
    public int MaoDeObraId { get; set; }
    public int OrcamentoId { get; set; }
    public int TecnicoId { get; set; }

    // Copiados no momento da inserção
    public string NomeTecnico { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public decimal ValorHora { get; set; }
    public decimal Horas { get; set; }
    public int NumeroTecnicos { get; set; } = 1; // 1 ou 2

    public decimal Subtotal => Horas * ValorHora * NumeroTecnicos;

    public Orcamento? Orcamento { get; set; }
    public Tecnico? Tecnico { get; set; }
}
