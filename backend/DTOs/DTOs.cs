namespace OrcamentosAPI.DTOs;

// ── Materiais ──────────────────────────────────────────────────────────────
public record MaterialDto(int MaterialId, string Nome, string Unidade, decimal PrecoUnitario, bool Ativo);
public record CreateMaterialDto(string Nome, string Unidade, decimal PrecoUnitario);
public record UpdateMaterialDto(string Nome, string Unidade, decimal PrecoUnitario, bool Ativo);

// ── Técnicos ───────────────────────────────────────────────────────────────
public record TecnicoDto(int TecnicoId, string Nome, string Cargo, decimal ValorHora, bool Ativo);
public record CreateTecnicoDto(string Nome, string Cargo, decimal ValorHora);
public record UpdateTecnicoDto(string Nome, string Cargo, decimal ValorHora, bool Ativo);

// ── Orçamentos ─────────────────────────────────────────────────────────────
public record OrcamentoResumoDto(
    int OrcamentoId,
    string NomeCliente,
    string DescricaoServico,
    string Status,
    DateTime CriadoEm,
    DateTime AtualizadoEm,
    decimal TotalMateriais,
    decimal TotalMaoDeObra,
    decimal TotalGeral,
    decimal HorasEstimadas
);

public record OrcamentoDetalheDto(
    int OrcamentoId,
    string NomeCliente,
    string DescricaoServico,
    string Status,
    DateTime CriadoEm,
    DateTime AtualizadoEm,
    List<ItemOrcamentoDto> Itens,
    List<MaoDeObraDto> MaoDeObra,
    decimal TotalMateriais,
    decimal TotalMaoDeObra,
    decimal TotalGeral,
    decimal HorasEstimadas
);

public record CreateOrcamentoDto(
    string NomeCliente,
    string DescricaoServico,
    List<CreateItemDto> Itens,
    List<CreateMaoDeObraDto> MaoDeObra
);

public record UpdateOrcamentoDto(
    string NomeCliente,
    string DescricaoServico,
    string Status,
    List<CreateItemDto> Itens,
    List<CreateMaoDeObraDto> MaoDeObra
);

// ── Itens ──────────────────────────────────────────────────────────────────
public record ItemOrcamentoDto(
    int ItemId,
    int MaterialId,
    string NomeMaterial,
    string Unidade,
    decimal PrecoUnitario,
    decimal Quantidade,
    decimal Subtotal
);

public record CreateItemDto(int MaterialId, decimal Quantidade);

// ── Mão de Obra ────────────────────────────────────────────────────────────
public record MaoDeObraDto(
    int MaoDeObraId,
    int TecnicoId,
    string NomeTecnico,
    string Cargo,
    decimal ValorHora,
    decimal Horas,
    int NumeroTecnicos,
    decimal Subtotal
);

public record CreateMaoDeObraDto(int TecnicoId, decimal Horas, int NumeroTecnicos);

// ── Dashboard ──────────────────────────────────────────────────────────────
public record DashboardDto(
    int TotalOrcamentos,
    decimal ValorTotalOrcado,
    decimal ValorTotalAprovado,
    int OrcamentosAprovados,
    int OrcamentosEnviados,
    int OrcamentosRascunho,
    int OrcamentosCancelados,
    List<OrcamentosPorMesDto> PorMes,
    List<MaterialMaisUsadoDto> MateriaisMaisUsados
);

public record OrcamentosPorMesDto(string Mes, int Quantidade, decimal Valor);
public record MaterialMaisUsadoDto(string Nome, decimal QuantidadeTotal);
