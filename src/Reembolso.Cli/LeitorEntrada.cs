using System.Globalization;
using System.Text.Json;
using Reembolso.Core;

namespace Reembolso.Cli;

public static class LeitorEntrada
{
    public static EntradaDespesas Ler(string caminho)
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(caminho));
        var raiz = doc.RootElement;

        var colaborador = raiz.GetProperty("colaborador");
        var competencia = ParseCompetencia(raiz.GetProperty("periodo").GetProperty("competencia").GetString()!);

        var despesas = raiz.GetProperty("despesas")
            .EnumerateArray()
            .Select(ParseDespesa)
            .ToList();

        return new EntradaDespesas(
            colaborador.GetProperty("id").GetString()!,
            TextoOpcional(colaborador, "nome"),
            TextoOpcional(colaborador, "centro_custo"),
            competencia,
            despesas);
    }

    private static Despesa ParseDespesa(JsonElement e) => new(
        e.GetProperty("id").GetString()!,
        DateOnly.ParseExact(e.GetProperty("data").GetString()!, "yyyy-MM-dd", CultureInfo.InvariantCulture),
        e.GetProperty("categoria").GetString()!,
        e.GetProperty("fornecedor").GetString()!,
        e.GetProperty("valor").GetDecimal(),
        e.GetProperty("tem_nota_fiscal").GetBoolean(),
        TextoOpcional(e, "descricao"));

    private static Competencia ParseCompetencia(string texto)
    {
        var d = DateOnly.ParseExact(texto + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
        return new Competencia(d.Year, d.Month);
    }

    private static string? TextoOpcional(JsonElement e, string nome) =>
        e.TryGetProperty(nome, out var v) ? v.GetString() : null;
}
