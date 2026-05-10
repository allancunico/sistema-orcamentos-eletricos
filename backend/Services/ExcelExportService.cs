using ClosedXML.Excel;
using OrcamentosAPI.Models;
using System.Globalization;

namespace OrcamentosAPI.Services;

public class ExcelExportService
{
    private const string VERDE = "6AAA1E";
    private const string VERDE_CLARO = "EBF5D6";
    private const string ESCURO = "222825";
    private const string CINZA = "F5F5F5";
    private const string BRANCO = "FFFFFF";
    private const string PRETO = "000000";

    private static readonly CultureInfo ptBR = new CultureInfo("pt-BR");
    private string Moeda(decimal v) => v.ToString("C2", ptBR);

    private void CenterV(IXLRange range) =>
        range.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

    private void CenterV(IXLCell cell) =>
        cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

    public byte[] Gerar(Orcamento orc)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Orçamento");
        ws.ShowGridLines = false;

        ws.Column(1).Width = 4.71;
        ws.Column(2).Width = 10.86;
        ws.Column(3).Width = 11.43;
        ws.Column(4).Width = 38.71;
        ws.Column(5).Width = 12.71;
        ws.Column(6).Width = 16.71;
        ws.Column(7).Width = 14.0;
        ws.Column(8).Width = 16.0;

        int row = 1;

        // ── Cabeçalho ─────────────────────────────────────────────────
        ws.Row(row).Height = 49.5;
        var r1 = ws.Range(row, 1, row, 8);
        r1.Merge(); r1.Value = "⚡ OrcaElétrico — Sistema de Orçamentos";
        r1.Style.Font.SetBold(true).Font.SetFontSize(19).Font.SetFontColor(XLColor.White)
            .Fill.SetBackgroundColor(XLColor.FromHtml(ESCURO))
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        row++;

        ws.Row(row).Height = 6;
        var r2 = ws.Range(row, 1, row, 8);
        r2.Merge(); r2.Style.Fill.SetBackgroundColor(XLColor.FromHtml(VERDE));
        row++;

        row++;

        // ── Info rows ─────────────────────────────────────────────────
        void InfoRow(string label, string valor)
        {
            ws.Row(row).Height = 21.75;
            var lbl = ws.Range(row, 2, row, 3);
            lbl.Merge(); lbl.Value = label;
            lbl.Style.Font.SetBold(true).Font.SetFontSize(13)
                .Font.SetFontColor(XLColor.FromHtml(ESCURO))
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            var val = ws.Range(row, 4, row, 8);
            val.Merge(); val.Value = valor;
            val.Style.Font.SetFontSize(13).Font.SetFontColor(XLColor.FromHtml(PRETO))
                .Fill.SetBackgroundColor(XLColor.FromHtml(CINZA))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Alignment.SetIndent(1);
            row++;
        }

        InfoRow("Nº do Orçamento:", $"ORC-{orc.OrcamentoId:D4}");
        InfoRow("Cliente:", orc.NomeCliente);
        InfoRow("Serviço:", orc.DescricaoServico);
        InfoRow("Data do Orçamento:", orc.CriadoEm.ToLocalTime().ToString("dd/MM/yyyy"));
        InfoRow("Status:", orc.Status);
        row++;

        // ── Seção Materiais ───────────────────────────────────────────
        ws.Row(row).Height = 27.75;
        var secMat = ws.Range(row, 1, row, 8);
        secMat.Merge(); secMat.Value = "  MATERIAIS";
        secMat.Style.Font.SetBold(true).Font.SetFontSize(14).Font.SetFontColor(XLColor.White)
            .Fill.SetBackgroundColor(XLColor.FromHtml(VERDE))
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        row++;

        ws.Row(row).Height = 21.75;
        void ThCell(int c1, int c2, string val)
        {
            var r = ws.Range(row, c1, row, c2);
            if (c1 != c2) r.Merge();
            r.Value = val;
            r.Style.Font.SetBold(true).Font.SetFontSize(12)
                .Font.SetFontColor(XLColor.FromHtml(ESCURO))
                .Fill.SetBackgroundColor(XLColor.FromHtml(VERDE_CLARO))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        }
        ThCell(2, 2, "#");
        ThCell(3, 4, "Descrição do Material");
        ThCell(5, 5, "Unid.");
        ThCell(6, 6, "Quantidade");
        ThCell(7, 7, "Preço Unit.");
        ThCell(8, 8, "Subtotal");
        row++;

        int seq = 1;
        foreach (var item in orc.Itens)
        {
            ws.Row(row).Height = 19.5;
            bool par = seq % 2 == 0;
            var bg = XLColor.FromHtml(par ? CINZA : BRANCO);

            ws.Cell(row, 2).Value = seq++;
            ws.Cell(row, 2).Style.Font.SetFontSize(13).Fill.SetBackgroundColor(bg)
                .Font.SetFontColor(XLColor.FromHtml(PRETO))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            var desc = ws.Range(row, 3, row, 4);
            desc.Merge(); desc.Value = item.NomeMaterial;
            desc.Style.Font.SetFontSize(13).Fill.SetBackgroundColor(bg)
                .Font.SetFontColor(XLColor.FromHtml(PRETO))
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws.Cell(row, 5).Value = item.Unidade;
            ws.Cell(row, 5).Style.Font.SetFontSize(13).Fill.SetBackgroundColor(bg)
                .Font.SetFontColor(XLColor.FromHtml(PRETO))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws.Cell(row, 6).Value = (double)item.Quantidade;
            ws.Cell(row, 6).Style.Font.SetFontSize(13).Fill.SetBackgroundColor(bg)
                .Font.SetFontColor(XLColor.FromHtml(PRETO))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .NumberFormat.Format = item.Unidade is "un" or "pct" ? "0" : "0.#";

            ws.Cell(row, 7).Value = (double)item.PrecoUnitario;
            ws.Cell(row, 7).Style.Font.SetFontSize(13).Fill.SetBackgroundColor(bg)
                .Font.SetFontColor(XLColor.FromHtml(PRETO))
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .NumberFormat.Format = "#,##0.00";

            ws.Cell(row, 8).Value = (double)item.Subtotal;
            ws.Cell(row, 8).Style.Font.SetBold(true).Font.SetFontSize(13).Fill.SetBackgroundColor(bg)
                .Font.SetFontColor(XLColor.FromHtml(PRETO))
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .NumberFormat.Format = "#,##0.00";
            row++;
        }

        ws.Row(row).Height = 24;
        var totMatLbl = ws.Range(row, 2, row, 7);
        totMatLbl.Merge(); totMatLbl.Value = "TOTAL DE MATERIAIS";
        totMatLbl.Style.Font.SetBold(true).Font.SetFontSize(13)
            .Font.SetFontColor(XLColor.FromHtml(ESCURO))
            .Fill.SetBackgroundColor(XLColor.FromHtml(VERDE_CLARO))
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
            .Alignment.SetIndent(2);
        ws.Cell(row, 8).Value = (double)orc.TotalMateriais;
        ws.Cell(row, 8).Style.Font.SetBold(true).Font.SetFontSize(13)
            .Font.SetFontColor(XLColor.FromHtml(ESCURO))
            .Fill.SetBackgroundColor(XLColor.FromHtml(VERDE_CLARO))
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
            .NumberFormat.Format = "#,##0.00";
        row++;
        row++;

        // ── Seção Mão de Obra ─────────────────────────────────────────
        ws.Row(row).Height = 27.75;
        var secMO = ws.Range(row, 1, row, 8);
        secMO.Merge(); secMO.Value = "  MÃO DE OBRA";
        secMO.Style.Font.SetBold(true).Font.SetFontSize(14).Font.SetFontColor(XLColor.White)
            .Fill.SetBackgroundColor(XLColor.FromHtml(VERDE))
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        row++;

        // Cabeçalho MO — coluna E colorida junto com o header
        ws.Row(row).Height = 21.75;
        void ThMO(int c1, int c2, string val)
        {
            var r = ws.Range(row, c1, row, c2);
            if (c1 != c2) r.Merge();
            r.Value = val;
            r.Style.Font.SetBold(true).Font.SetFontSize(12)
                .Font.SetFontColor(XLColor.FromHtml(ESCURO))
                .Fill.SetBackgroundColor(XLColor.FromHtml(VERDE_CLARO))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        }
        ThMO(2, 2, "#");
        ThMO(3, 5, "Técnico / Cargo"); // C até E merged — cobre coluna E
        ThMO(6, 6, "Horas");
        ThMO(7, 7, "Valor/Hora");
        ThMO(8, 8, "Subtotal");
        row++;

        seq = 1;
        foreach (var mo in orc.MaoDeObra)
        {
            ws.Row(row).Height = 19.5;
            bool par = seq % 2 == 0;
            var bg = XLColor.FromHtml(par ? CINZA : BRANCO);

            ws.Cell(row, 2).Value = seq++;
            ws.Cell(row, 2).Style.Font.SetFontSize(13).Fill.SetBackgroundColor(bg)
                .Font.SetFontColor(XLColor.FromHtml(PRETO))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            // Nome do técnico — merge C até E para cobrir coluna E
            var nome = ws.Range(row, 3, row, 5);
            nome.Merge(); nome.Value = $"{mo.NomeTecnico} — {mo.Cargo}";
            nome.Style.Font.SetFontSize(13).Fill.SetBackgroundColor(bg)
                .Font.SetFontColor(XLColor.FromHtml(PRETO))
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws.Cell(row, 6).Value = $"{mo.Horas:0.#}h";
            ws.Cell(row, 6).Style.Font.SetFontSize(13).Fill.SetBackgroundColor(bg)
                .Font.SetFontColor(XLColor.FromHtml(PRETO))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            ws.Cell(row, 7).Value = (double)mo.ValorHora;
            ws.Cell(row, 7).Style.Font.SetFontSize(13).Fill.SetBackgroundColor(bg)
                .Font.SetFontColor(XLColor.FromHtml(PRETO))
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .NumberFormat.Format = "#,##0.00";

            ws.Cell(row, 8).Value = (double)mo.Subtotal;
            ws.Cell(row, 8).Style.Font.SetBold(true).Font.SetFontSize(13).Fill.SetBackgroundColor(bg)
                .Font.SetFontColor(XLColor.FromHtml(PRETO))
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .NumberFormat.Format = "#,##0.00";
            row++;
        }

        ws.Row(row).Height = 24;
        var totMOLbl = ws.Range(row, 2, row, 7);
        totMOLbl.Merge(); totMOLbl.Value = "TOTAL DE MÃO DE OBRA";
        totMOLbl.Style.Font.SetBold(true).Font.SetFontSize(13)
            .Font.SetFontColor(XLColor.FromHtml(ESCURO))
            .Fill.SetBackgroundColor(XLColor.FromHtml(VERDE_CLARO))
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
            .Alignment.SetIndent(2);
        ws.Cell(row, 8).Value = (double)orc.TotalMaoDeObra;
        ws.Cell(row, 8).Style.Font.SetBold(true).Font.SetFontSize(13)
            .Font.SetFontColor(XLColor.FromHtml(ESCURO))
            .Fill.SetBackgroundColor(XLColor.FromHtml(VERDE_CLARO))
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
            .NumberFormat.Format = "#,##0.00";
        row++;
        row++;

        // ── Resumo ────────────────────────────────────────────────────
        ws.Row(row).Height = 27.75;
        var secRes = ws.Range(row, 1, row, 8);
        secRes.Merge(); secRes.Value = "  RESUMO DO ORÇAMENTO";
        secRes.Style.Font.SetBold(true).Font.SetFontSize(14).Font.SetFontColor(XLColor.White)
            .Fill.SetBackgroundColor(XLColor.FromHtml(ESCURO))
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        row++;

        void ResRow(string label, string valor)
        {
            ws.Row(row).Height = 21.75;
            var lbl = ws.Range(row, 2, row, 7);
            lbl.Merge(); lbl.Value = label;
            lbl.Style.Font.SetFontSize(13).Font.SetFontColor(XLColor.FromHtml(ESCURO))
                .Fill.SetBackgroundColor(XLColor.FromHtml(CINZA))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Alignment.SetIndent(2);
            ws.Cell(row, 8).Value = valor;
            ws.Cell(row, 8).Style.Font.SetFontSize(13).Font.SetFontColor(XLColor.FromHtml(ESCURO))
                .Fill.SetBackgroundColor(XLColor.FromHtml(CINZA))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            row++;
        }

        ResRow("Custo de Materiais", Moeda(orc.TotalMateriais));
        ResRow("Custo de Mão de Obra", Moeda(orc.TotalMaoDeObra));
        ResRow("Tempo Estimado", $"{orc.HorasEstimadas:0.#} horas");

        ws.Row(row).Height = 31.5;
        var totGLbl = ws.Range(row, 1, row, 7);
        totGLbl.Merge(); totGLbl.Value = "TOTAL GERAL DO ORÇAMENTO";
        totGLbl.Style.Font.SetBold(true).Font.SetFontSize(15).Font.SetFontColor(XLColor.White)
            .Fill.SetBackgroundColor(XLColor.FromHtml(VERDE))
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
            .Alignment.SetIndent(2);
        ws.Cell(row, 8).Value = Moeda(orc.TotalGeral);
        ws.Cell(row, 8).Style.Font.SetBold(true).Font.SetFontSize(15).Font.SetFontColor(XLColor.White)
            .Fill.SetBackgroundColor(XLColor.FromHtml(VERDE))
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        row++;

        ws.Row(row).Height = 18;
        var rodape = ws.Range(row, 1, row, 8);
        rodape.Merge();
        rodape.Value = $"Documento gerado em {DateTime.Now:dd/MM/yyyy 'às' HH:mm} — OrcaElétrico";
        rodape.Style.Font.SetFontSize(9).Font.SetItalic(true).Font.SetFontColor(XLColor.Gray)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}