using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrcamentosAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Materiais",
                columns: table => new
                {
                    MaterialId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Unidade = table.Column<string>(type: "text", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materiais", x => x.MaterialId);
                });

            migrationBuilder.CreateTable(
                name: "Orcamentos",
                columns: table => new
                {
                    OrcamentoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeCliente = table.Column<string>(type: "text", nullable: false),
                    DescricaoServico = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orcamentos", x => x.OrcamentoId);
                });

            migrationBuilder.CreateTable(
                name: "Tecnicos",
                columns: table => new
                {
                    TecnicoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Cargo = table.Column<string>(type: "text", nullable: false),
                    ValorHora = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnicos", x => x.TecnicoId);
                });

            migrationBuilder.CreateTable(
                name: "ItensOrcamento",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrcamentoId = table.Column<int>(type: "integer", nullable: false),
                    MaterialId = table.Column<int>(type: "integer", nullable: false),
                    NomeMaterial = table.Column<string>(type: "text", nullable: false),
                    Unidade = table.Column<string>(type: "text", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Quantidade = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensOrcamento", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_ItensOrcamento_Materiais_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materiais",
                        principalColumn: "MaterialId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensOrcamento_Orcamentos_OrcamentoId",
                        column: x => x.OrcamentoId,
                        principalTable: "Orcamentos",
                        principalColumn: "OrcamentoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaoDeObra",
                columns: table => new
                {
                    MaoDeObraId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrcamentoId = table.Column<int>(type: "integer", nullable: false),
                    TecnicoId = table.Column<int>(type: "integer", nullable: false),
                    NomeTecnico = table.Column<string>(type: "text", nullable: false),
                    Cargo = table.Column<string>(type: "text", nullable: false),
                    ValorHora = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Horas = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    NumeroTecnicos = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaoDeObra", x => x.MaoDeObraId);
                    table.ForeignKey(
                        name: "FK_MaoDeObra_Orcamentos_OrcamentoId",
                        column: x => x.OrcamentoId,
                        principalTable: "Orcamentos",
                        principalColumn: "OrcamentoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaoDeObra_Tecnicos_TecnicoId",
                        column: x => x.TecnicoId,
                        principalTable: "Tecnicos",
                        principalColumn: "TecnicoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Materiais",
                columns: new[] { "MaterialId", "Ativo", "CriadoEm", "Nome", "PrecoUnitario", "Unidade" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Disjuntor Monofasico 10A", 18.50m, "un" },
                    { 2, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Disjuntor Monofasico 20A", 19.80m, "un" },
                    { 3, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Disjuntor Monofasico 30A", 21.00m, "un" },
                    { 4, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Disjuntor Bifasico 20A", 45.00m, "un" },
                    { 5, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Disjuntor Trifasico 40A", 89.00m, "un" },
                    { 6, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Disjuntor DR 25A/30mA", 95.00m, "un" },
                    { 7, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Barramento de cobre 100A", 35.00m, "un" },
                    { 8, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fio 2,5mm", 3.80m, "m" },
                    { 9, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fio 4mm", 5.90m, "m" },
                    { 10, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fio 6mm", 8.50m, "m" },
                    { 11, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fio 10mm", 14.00m, "m" },
                    { 12, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cabo PP 3x2,5mm", 12.00m, "m" },
                    { 13, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Quadro distribuicao 12 disjuntores", 85.00m, "un" },
                    { 14, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Quadro distribuicao 24 disjuntores", 145.00m, "un" },
                    { 15, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tomada 2P+T 20A", 12.00m, "un" },
                    { 16, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tomada 2P+T 10A", 8.50m, "un" },
                    { 17, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Interruptor simples", 7.00m, "un" },
                    { 18, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Eletroduto 3/4", 4.20m, "m" },
                    { 19, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Abracadeira plastica pct 100", 9.00m, "pct" },
                    { 20, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Terminal ilhos 2,5mm", 1.20m, "un" },
                    { 21, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fita isolante 18m", 5.50m, "un" }
                });

            migrationBuilder.InsertData(
                table: "Tecnicos",
                columns: new[] { "TecnicoId", "Ativo", "Cargo", "Nome", "ValorHora" },
                values: new object[,]
                {
                    { 1, true, "Tecnico Senior", "Carlos Silva", 120.00m },
                    { 2, true, "Tecnico Junior", "Ana Souza", 80.00m },
                    { 3, true, "Engenheiro", "Roberto Lima", 200.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensOrcamento_MaterialId",
                table: "ItensOrcamento",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensOrcamento_OrcamentoId",
                table: "ItensOrcamento",
                column: "OrcamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_MaoDeObra_OrcamentoId",
                table: "MaoDeObra",
                column: "OrcamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_MaoDeObra_TecnicoId",
                table: "MaoDeObra",
                column: "TecnicoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensOrcamento");

            migrationBuilder.DropTable(
                name: "MaoDeObra");

            migrationBuilder.DropTable(
                name: "Materiais");

            migrationBuilder.DropTable(
                name: "Orcamentos");

            migrationBuilder.DropTable(
                name: "Tecnicos");
        }
    }
}
