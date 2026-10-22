using Reembolso.Cli;
using Reembolso.Core;

namespace Reembolso.Tests;

public class LeituraEntradaTests
{
    [Fact]
    public void SEC4_LeExemploOficial_Retorna14DespesasNaOrdemDoArquivo()
    {
        var entrada = LeitorEntrada.Ler(CaminhoDoExemplo());

        Assert.Equal("c-0417", entrada.ColaboradorId);
        Assert.Equal(new Competencia(2026, 7), entrada.Competencia);
        Assert.Equal(14, entrada.Despesas.Count);
        Assert.Equal(
            Enumerable.Range(1, 14).Select(n => $"d-{n:000}"),
            entrada.Despesas.Select(d => d.Id));

        var d011 = entrada.Despesas[10];
        Assert.Equal(33.333m, d011.Valor);
        Assert.Equal(new DateOnly(2026, 7, 15), d011.Data);
        Assert.True(d011.TemNotaFiscal);
    }

    private static string CaminhoDoExemplo()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Reembolso.sln")))
            dir = dir.Parent;
        return Path.Combine(dir!.FullName, "exemplos", "despesas-exemplo.json");
    }
}
