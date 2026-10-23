using Reembolso.Cli;
using Reembolso.Core;

namespace Reembolso.Tests;

public class ViagemTests
{
    [Fact]
    public void RN006_ArquivoCompleto_ViagemVerdadeiro()
    {
        var resultado = AplicarAoExemplo(semIds: []);

        Assert.True(resultado.Viagem);
    }

    [Fact]
    public void RN006_SemD010ESemD013_ViagemFalso()
    {
        var resultado = AplicarAoExemplo(semIds: ["d-010", "d-013"]);

        Assert.False(resultado.Viagem);
    }

    [Fact]
    public void RN006_D013UnicaHospedagem_ViagemFalsoPoisRecusada()
    {
        var resultado = AplicarAoExemplo(semIds: ["d-010"]);

        var d013 = resultado.Itens.Single(i => i.Despesa.Id == "d-013");
        Assert.Equal(Situacao.Recusado, d013.Situacao);
        Assert.Contains("RN-004", d013.Regras);
        Assert.False(resultado.Viagem);
    }

    [Fact]
    public void AMB014_HospedagemEmRevisao_NaoCaracterizaViagem()
    {
        // Duas hospedagens idênticas viram duplicata (RN-005) e vão para revisão.
        var contexto = Contexto(
            Item("d-a", categoria: "hospedagem", valor: 200m),
            Item("d-b", categoria: "hospedagem", valor: 200m));

        var resultado = new Viagem().Aplicar(new Duplicatas().Aplicar(contexto));

        Assert.All(resultado.Itens, i => Assert.Equal(Situacao.EmRevisao, i.Situacao));
        Assert.False(resultado.Viagem);
    }

    [Fact]
    public void AMB006_LimiarDeNotaEHospedagemNaoMudamEmViagem()
    {
        var contexto = Contexto(
            Item("d-hotel", categoria: "hospedagem", valor: 200m),
            Item("d-taxi", categoria: "transporte_urbano", valor: 100.01m, temNota: false));

        var aposNota = new NotaFiscalObrigatoria().Aplicar(contexto);
        var resultado = new Viagem().Aplicar(aposNota);

        Assert.True(resultado.Viagem);

        // O limiar de nota segue em 100,00 mesmo em viagem.
        // O limite de hospedagem (250,00) e os tetos 90/120 são testados na T-009 e na T-010.
        var taxi = resultado.Itens.Single(i => i.Despesa.Id == "d-taxi");
        Assert.Equal(Situacao.Recusado, taxi.Situacao);
        Assert.Contains("RN-004", taxi.Regras);
    }

    private static ItemAvaliado Item(
        string id,
        string categoria,
        decimal valor,
        bool temNota = true)
    {
        var despesa = new Despesa(id, new DateOnly(2026, 7, 14), categoria, "Hotel Copa Sul", valor, temNota, "Diaria");
        return ItemAvaliado.De(despesa);
    }

    private static ContextoAvaliacao Contexto(params ItemAvaliado[] itens) =>
        new(new Competencia(2026, 7), false, itens);

    private static ContextoAvaliacao AplicarAoExemplo(string[] semIds)
    {
        var entrada = LeitorEntrada.Ler(CaminhoDoExemplo());
        var itens = entrada.Despesas
            .Where(d => !semIds.Contains(d.Id))
            .Select(ItemAvaliado.De)
            .ToList();
        var contexto = new ContextoAvaliacao(entrada.Competencia, false, itens);

        contexto = new CategoriaNormalizada().Aplicar(contexto);
        contexto = new PeriodoCompetencia().Aplicar(contexto);
        contexto = new ValorNegativo().Aplicar(contexto);
        contexto = new NotaFiscalObrigatoria().Aplicar(contexto);
        contexto = new Duplicatas().Aplicar(contexto);

        return new Viagem().Aplicar(contexto);
    }

    private static string CaminhoDoExemplo()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Reembolso.sln")))
            dir = dir.Parent;
        return Path.Combine(dir!.FullName, "exemplos", "despesas-exemplo.json");
    }
}
