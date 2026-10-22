using Reembolso.Cli;
using Reembolso.Core;

namespace Reembolso.Tests;

public class PeriodoCompetenciaTests
{
    [Fact]
    public void RN003_D008ForaDaCompetencia_EmRevisaoComZero()
    {
        var contexto = ContextoDoExemplo();

        var resultado = new PeriodoCompetencia().Aplicar(contexto);

        var d008 = resultado.Itens.Single(i => i.Despesa.Id == "d-008");
        Assert.Equal(Situacao.EmRevisao, d008.Situacao);
        Assert.Equal(0m, d008.ValorReembolsavel);
        Assert.Contains("RN-003", d008.Regras);
    }

    [Fact]
    public void RN003_D014UltimoDiaDoMes_Dentro()
    {
        var contexto = ContextoDoExemplo();

        var resultado = new PeriodoCompetencia().Aplicar(contexto);

        var d014 = resultado.Itens.Single(i => i.Despesa.Id == "d-014");
        Assert.Equal(new DateOnly(2026, 7, 31), d014.Despesa.Data);
        Assert.Equal(Situacao.Ativo, d014.Situacao);
    }

    [Fact]
    public void RN003_PrimeiroDiaDoMes_Dentro()
    {
        var resultado = Aplicar(new DateOnly(2026, 7, 1));

        Assert.Equal(Situacao.Ativo, resultado.Situacao);
    }

    [Theory]
    [InlineData(2026, 6, 30)]
    [InlineData(2026, 8, 1)]
    [InlineData(2025, 7, 15)]
    [InlineData(2027, 7, 15)]
    public void AMB011_ComparaPorAnoEMesDaCompetencia(int ano, int mes, int dia)
    {
        var resultado = Aplicar(new DateOnly(ano, mes, dia));

        Assert.Equal(Situacao.EmRevisao, resultado.Situacao);
        Assert.Equal(0m, resultado.ValorReembolsavel);
    }

    [Fact]
    public void AMB016_ComparaPelaDataDaDespesa()
    {
        var despesa = new Despesa("d-x", new DateOnly(2026, 4, 15), "alimentacao", "Fornecedor", 41m, true, null);
        var contexto = new ContextoAvaliacao(new Competencia(2026, 7), false, [ItemAvaliado.De(despesa)]);

        var resultado = new PeriodoCompetencia().Aplicar(contexto).Itens.Single();

        Assert.Equal(Situacao.EmRevisao, resultado.Situacao);
        Assert.Contains("RN-003", resultado.Regras);
    }

    private static ItemAvaliado Aplicar(DateOnly data)
    {
        var despesa = new Despesa("d-x", data, "alimentacao", "Fornecedor", 50m, true, null);
        var contexto = new ContextoAvaliacao(
            new Competencia(2026, 7), false, [ItemAvaliado.De(despesa)]);

        return new PeriodoCompetencia().Aplicar(contexto).Itens.Single();
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
