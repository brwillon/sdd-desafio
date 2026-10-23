using Reembolso.Cli;
using Reembolso.Core;

namespace Reembolso.Tests;

public class LimiteHospedagemDiariaTests
{
    private static readonly DateOnly Dia = new(2026, 7, 14);

    [Fact]
    public void RN008_D010_Paga250ECorta230()
    {
        var resultado = AplicarAoExemplo();

        var d010 = Item(resultado, "d-010");
        Assert.Equal(480.00m, d010.Despesa.Valor);
        Assert.Equal(250.00m, d010.ValorReembolsavel);
        Assert.Equal(230.00m, d010.Despesa.Valor - d010.ValorReembolsavel);
    }

    [Fact]
    public void RN008_D013TresNoites_RecusadaPorRN004AntesDoLimite()
    {
        var resultado = AplicarAoExemplo();

        var d013 = Item(resultado, "d-013");
        Assert.Equal("hospedagem", d013.Categoria);
        Assert.Equal(Situacao.Recusado, d013.Situacao);
        Assert.Equal(0m, d013.ValorReembolsavel);
        Assert.Contains("RN-004", d013.Regras);
    }

    [Fact]
    public void AMB004_LimiteDe250EhPorLancamento()
    {
        // Mesmo dia, mesma categoria: não há saldo diário compartilhado como na RN-007.
        var resultado = Aplicar(Contexto(
            Item("d-a", Dia, 300m, "Hotel Copa Sul", "Diaria 1"),
            Item("d-b", Dia, 400m, "Hotel Copa Sul", "Diaria 2")));

        Assert.Equal(250.00m, Reembolsavel(resultado, "d-a"));
        Assert.Equal(250.00m, Reembolsavel(resultado, "d-b"));
    }

    [Fact]
    public void AMB010_Hospedagem250Exato_PagaIntegral()
    {
        var resultado = Aplicar(Contexto(
            Item("d-a", Dia, 250m, "Hotel Copa Sul", "Diaria")));

        var item = Item(resultado, "d-a");
        Assert.Equal(250.00m, item.ValorReembolsavel);
        Assert.Equal(Situacao.Aprovado, item.Situacao);
    }

    [Fact]
    public void AMB015_DescricaoComDuasDiarias_ContaComoUma()
    {
        // Se a quantidade da descrição fosse usada, 2 x 250 = 500 pagaria 500,00.
        var resultado = Aplicar(Contexto(
            Item("d-a", Dia, 480m, "Hotel Copa Sul", "Hotel Rio - 2 diarias"),
            Item("d-b", Dia.AddDays(5), 690m, "Pousada Sol", "3 noites")));

        Assert.Equal(250.00m, Reembolsavel(resultado, "d-a"));
        Assert.Equal(250.00m, Reembolsavel(resultado, "d-b"));
    }

    [Fact]
    public void AMB024_DiariasLancadasEmDatasDistintas_CadaUmaLimitadaA250()
    {
        var resultado = Aplicar(Contexto(
            Item("d-a", Dia, 300m, "Hotel Copa Sul", "Diaria 1 de 3"),
            Item("d-b", Dia.AddDays(1), 300m, "Hotel Copa Sul", "Diaria 2 de 3"),
            Item("d-c", Dia.AddDays(2), 200m, "Hotel Copa Sul", "Diaria 3 de 3")));

        Assert.Equal(250.00m, Reembolsavel(resultado, "d-a"));
        Assert.Equal(250.00m, Reembolsavel(resultado, "d-b"));
        Assert.Equal(200.00m, Reembolsavel(resultado, "d-c"));
    }

    private static ItemAvaliado Item(
        string id,
        DateOnly data,
        decimal valor,
        string fornecedor,
        string descricao)
    {
        var despesa = new Despesa(id, data, "hospedagem", fornecedor, valor, true, descricao);
        return ItemAvaliado.De(despesa);
    }

    private static ContextoAvaliacao Contexto(params ItemAvaliado[] itens) =>
        new(new Competencia(2026, 7), false, itens);

    private static ContextoAvaliacao Aplicar(ContextoAvaliacao contexto)
    {
        contexto = new Duplicatas().Aplicar(contexto);
        contexto = new Viagem().Aplicar(contexto);
        return new LimiteHospedagemDiaria().Aplicar(contexto);
    }

    private static ContextoAvaliacao AplicarAoExemplo()
    {
        var entrada = LeitorEntrada.Ler(CaminhoDoExemplo());
        var itens = entrada.Despesas.Select(ItemAvaliado.De).ToList();
        var contexto = new ContextoAvaliacao(entrada.Competencia, false, itens);

        contexto = new CategoriaNormalizada().Aplicar(contexto);
        contexto = new PeriodoCompetencia().Aplicar(contexto);
        contexto = new ValorNegativo().Aplicar(contexto);
        contexto = new NotaFiscalObrigatoria().Aplicar(contexto);
        contexto = new Duplicatas().Aplicar(contexto);
        contexto = new Viagem().Aplicar(contexto);
        contexto = new LimiteDiario().Aplicar(contexto);

        return new LimiteHospedagemDiaria().Aplicar(contexto);
    }

    private static ItemAvaliado Item(ContextoAvaliacao resultado, string id) =>
        resultado.Itens.Single(i => i.Despesa.Id == id);

    private static decimal Reembolsavel(ContextoAvaliacao resultado, string id) =>
        Item(resultado, id).ValorReembolsavel;

    private static string CaminhoDoExemplo()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Reembolso.sln")))
            dir = dir.Parent;
        return Path.Combine(dir!.FullName, "exemplos", "despesas-exemplo.json");
    }
}
