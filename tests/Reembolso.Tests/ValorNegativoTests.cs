using Reembolso.Cli;
using Reembolso.Core;

namespace Reembolso.Tests;

public class ValorNegativoTests
{
    [Fact]
    public void RN002_D009Negativo_RecusadoComZero()
    {
        var contexto = ContextoDoExemplo();

        var resultado = new ValorNegativo().Aplicar(contexto);

        var d009 = resultado.Itens.Single(i => i.Despesa.Id == "d-009");
        Assert.Equal(Situacao.Recusado, d009.Situacao);
        Assert.Equal(0m, d009.ValorReembolsavel);
        Assert.Contains("RN-002", d009.Regras);
    }

    [Fact]
    public void RN002_D009_NaoAlteraSaldoNemTotalDosOutros()
    {
        var contexto = ContextoDoExemplo();
        var semD009 = contexto with
        {
            Itens = contexto.Itens.Where(i => i.Despesa.Id != "d-009").ToList()
        };

        var resultado = new ValorNegativo().Aplicar(contexto);

        var outrosCom = resultado.Itens.Where(i => i.Despesa.Id != "d-009").ToList();
        var outrosSem = new ValorNegativo().Aplicar(semD009).Itens;
        Assert.Equal(outrosSem, outrosCom);
        Assert.Equal(outrosSem.Sum(i => i.ValorReembolsavel), outrosCom.Sum(i => i.ValorReembolsavel));
    }

    [Fact]
    public void AMB017_EstornoRecusado_ApenasOItem()
    {
        var contexto = ContextoDoExemplo();

        var resultado = new ValorNegativo().Aplicar(contexto);

        Assert.Equal(contexto.Itens.Count, resultado.Itens.Count);
        var recusados = resultado.Itens.Where(i => i.Situacao == Situacao.Recusado).ToList();
        Assert.Equal(["d-009"], recusados.Select(i => i.Despesa.Id));
        Assert.All(
            resultado.Itens.Where(i => i.Despesa.Id != "d-009"),
            i => Assert.Equal(Situacao.Ativo, i.Situacao));
    }

    private static ContextoAvaliacao ContextoDoExemplo()
    {
        var entrada = LeitorEntrada.Ler(CaminhoDoExemplo());
        var itens = entrada.Despesas.Select(ItemAvaliado.De).ToList();
        return new ContextoAvaliacao(entrada.Competencia, false, itens);
    }

    private static string CaminhoDoExemplo()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Reembolso.sln")))
            dir = dir.Parent;
        return Path.Combine(dir!.FullName, "exemplos", "despesas-exemplo.json");
    }
}
