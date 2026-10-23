using Reembolso.Cli;
using Reembolso.Core;

namespace Reembolso.Tests;

public class LimiteDiarioTests
{
    private static readonly DateOnly Dia = new(2026, 7, 14);

    [Fact]
    public void RN007_D001D002EmViagem_Pagam7250E1750()
    {
        var resultado = AplicarAoExemplo(semIds: []);

        Assert.Equal(72.50m, Reembolsavel(resultado, "d-001"));
        Assert.Equal(17.50m, Reembolsavel(resultado, "d-002"));
    }

    [Fact]
    public void RN007_D001D002SemViagem_Pagam6000E0Recusado()
    {
        var resultado = AplicarAoExemplo(
            semIds: ["d-003", "d-004", "d-005", "d-006", "d-007", "d-008", "d-009",
                     "d-010", "d-011", "d-012", "d-013", "d-014"]);

        Assert.False(resultado.Viagem);
        Assert.Equal(60.00m, Reembolsavel(resultado, "d-001"));
        Assert.Equal(0m, Reembolsavel(resultado, "d-002"));
        Assert.Equal(Situacao.Recusado, Item(resultado, "d-002").Situacao);
        Assert.Contains("RN-007", Item(resultado, "d-002").Regras);
    }

    [Fact]
    public void RN007_D003Sozinha_Paga80()
    {
        var resultado = AplicarAoExemplo(semIds: IdsExceto("d-003"));

        Assert.False(resultado.Viagem);
        Assert.Equal(80.00m, Reembolsavel(resultado, "d-003"));
    }

    [Fact]
    public void RN007_D014Sozinha_Paga60()
    {
        var resultado = AplicarAoExemplo(semIds: IdsExceto("d-014"));

        Assert.False(resultado.Viagem);
        Assert.Equal(60.00m, Reembolsavel(resultado, "d-014"));
    }

    [Fact]
    public void RN007_ArquivoCompleto_D003Paga100_D011Paga3333_D012Paga4720_D014Paga6100()
    {
        var resultado = AplicarAoExemplo(semIds: []);

        Assert.True(resultado.Viagem);
        Assert.Equal(100.00m, Reembolsavel(resultado, "d-003"));
        Assert.Equal(33.33m, Reembolsavel(resultado, "d-011"));
        Assert.Equal(47.20m, Reembolsavel(resultado, "d-012"));
        Assert.Equal(61.00m, Reembolsavel(resultado, "d-014"));
    }

    [Fact]
    public void RN010_EmRevisao_NaoConsomeSaldoDiario()
    {
        // d-a e d-b são duplicatas (RN-005) e vão para revisão; d-c recebe o saldo cheio.
        var contexto = Contexto(
            Item("d-a", Dia, "alimentacao", 54.90m, "Bistro Central"),
            Item("d-b", Dia, "alimentacao", 54.90m, "Bistro Central"),
            Item("d-c", Dia, "alimentacao", 50.00m, "Cantina do Porto"));

        var resultado = Aplicar(contexto);

        Assert.Equal(Situacao.EmRevisao, Item(resultado, "d-a").Situacao);
        Assert.Equal(Situacao.EmRevisao, Item(resultado, "d-b").Situacao);
        Assert.Equal(50.00m, Reembolsavel(resultado, "d-c"));
    }

    [Fact]
    public void RN006_Viagem_AmpliaLimitesDeAlimentacaoETransporte()
    {
        var itens = new[]
        {
            Item("d-comida", Dia, "alimentacao", 200m, "Restaurante A"),
            Item("d-taxi", Dia, "transporte_urbano", 200m, "TaxiApp", temNota: true),
        };

        var semViagem = Aplicar(Contexto(itens));
        var emViagem = Aplicar(Contexto(
            [.. itens, Item("d-hotel", Dia, "hospedagem", 200m, "Hotel Copa Sul")]));

        Assert.False(semViagem.Viagem);
        Assert.Equal(60.00m, Reembolsavel(semViagem, "d-comida"));
        Assert.Equal(80.00m, Reembolsavel(semViagem, "d-taxi"));
        Assert.True(emViagem.Viagem);
        Assert.Equal(90.00m, Reembolsavel(emViagem, "d-comida"));
        Assert.Equal(120.00m, Reembolsavel(emViagem, "d-taxi"));
    }

    [Fact]
    public void AMB001_SaldoPorDiaECategoria_NaoPorDespesa()
    {
        // Cada despesa isolada cabe em 60,00; só a soma do dia estoura.
        var contexto = Contexto(
            Item("d-a", Dia, "alimentacao", 40m, "Restaurante A"),
            Item("d-b", Dia, "alimentacao", 40m, "Restaurante B"));

        var resultado = Aplicar(contexto);

        Assert.Equal(40.00m, Reembolsavel(resultado, "d-a"));
        Assert.Equal(20.00m, Reembolsavel(resultado, "d-b"));
    }

    [Fact]
    public void AMB001_AlimentacaoETransporteNaoCompartilhamSaldo()
    {
        var contexto = Contexto(
            Item("d-comida", Dia, "alimentacao", 60m, "Restaurante A"),
            Item("d-taxi", Dia, "transporte_urbano", 80m, "TaxiApp"));

        var resultado = Aplicar(contexto);

        Assert.Equal(60.00m, Reembolsavel(resultado, "d-comida"));
        Assert.Equal(80.00m, Reembolsavel(resultado, "d-taxi"));
    }

    [Fact]
    public void AMB001_SaldoNaoPassaDeUmDiaParaOutro()
    {
        var contexto = Contexto(
            Item("d-a", Dia, "alimentacao", 60m, "Restaurante A"),
            Item("d-b", Dia.AddDays(1), "alimentacao", 60m, "Restaurante A"));

        var resultado = Aplicar(contexto);

        Assert.Equal(60.00m, Reembolsavel(resultado, "d-a"));
        Assert.Equal(60.00m, Reembolsavel(resultado, "d-b"));
    }

    [Fact]
    public void AMB002_CorteSegueAOrdemDoArquivo()
    {
        var primeiroMaior = Aplicar(Contexto(
            Item("d-a", Dia, "alimentacao", 50m, "Restaurante A"),
            Item("d-b", Dia, "alimentacao", 30m, "Restaurante B")));
        var primeiroMenor = Aplicar(Contexto(
            Item("d-b", Dia, "alimentacao", 30m, "Restaurante B"),
            Item("d-a", Dia, "alimentacao", 50m, "Restaurante A")));

        Assert.Equal(50.00m, Reembolsavel(primeiroMaior, "d-a"));
        Assert.Equal(10.00m, Reembolsavel(primeiroMaior, "d-b"));
        Assert.Equal(30.00m, Reembolsavel(primeiroMenor, "d-b"));
        Assert.Equal(30.00m, Reembolsavel(primeiroMenor, "d-a"));
    }

    [Fact]
    public void AMB003_PagaOLimiteECortaOExcedente()
    {
        var resultado = Aplicar(Contexto(
            Item("d-a", Dia, "alimentacao", 100m, "Restaurante A")));

        var item = Item(resultado, "d-a");
        Assert.Equal(60.00m, item.ValorReembolsavel);
        Assert.Equal(Situacao.Aprovado, item.Situacao);
        Assert.Contains("RN-007", item.Regras);
    }

    [Fact]
    public void AMB010_AlimentacaoIgualAoLimite60_PagaIntegral()
    {
        var resultado = Aplicar(Contexto(
            Item("d-a", Dia, "alimentacao", 60m, "Restaurante A")));

        var item = Item(resultado, "d-a");
        Assert.Equal(60.00m, item.ValorReembolsavel);
        Assert.Equal(Situacao.Aprovado, item.Situacao);
    }

    [Fact]
    public void AMB019_D012Sabado_SemTratamentoEspecial()
    {
        var resultado = AplicarAoExemplo(semIds: []);

        var d012 = Item(resultado, "d-012");
        Assert.Equal(DayOfWeek.Saturday, d012.Despesa.Data.DayOfWeek);
        Assert.Equal(Situacao.Aprovado, d012.Situacao);
        Assert.Equal(47.20m, d012.ValorReembolsavel);

        // Um sábado acima do limite é cortado no limite normal, como um dia útil.
        var sabado = new DateOnly(2026, 7, 18);
        var corte = Aplicar(Contexto(
            Item("d-sab", sabado, "alimentacao", 100m, "Padaria Uniao"),
            Item("d-seg", sabado.AddDays(2), "alimentacao", 100m, "Padaria Uniao")));

        Assert.Equal(DayOfWeek.Saturday, sabado.DayOfWeek);
        Assert.Equal(60.00m, Reembolsavel(corte, "d-sab"));
        Assert.Equal(60.00m, Reembolsavel(corte, "d-seg"));
    }

    [Fact]
    public void AMB022_D001AlmocoComCliente_AprovadoNormalmente()
    {
        var resultado = AplicarAoExemplo(semIds: []);

        var d001 = Item(resultado, "d-001");
        Assert.Equal("Almoco com cliente", d001.Despesa.Descricao);
        Assert.Equal(Situacao.Aprovado, d001.Situacao);
        Assert.Equal(72.50m, d001.ValorReembolsavel);
        Assert.Empty(d001.Alertas);
    }

    [Fact]
    public void AMB023_D011CafeDaManha_EntraNoLimiteDeAlimentacao()
    {
        // Com d-010 (hospedagem) no arquivo há viagem: limite de 90,00 no dia 15.
        var resultado = AplicarAoExemplo(semIds: []);
        Assert.Equal(33.33m, Reembolsavel(resultado, "d-011"));

        // O café consome saldo: outro lançamento de alimentação no mesmo dia só recebe o resto.
        var comOutro = Aplicar(Contexto(
            Item("d-hotel", new DateOnly(2026, 7, 14), "hospedagem", 250m, "Hotel Copa Sul"),
            Item("d-cafe", new DateOnly(2026, 7, 15), "alimentacao", 33.33m, "Hotel Copa Sul"),
            Item("d-extra", new DateOnly(2026, 7, 15), "alimentacao", 80m, "Padaria Uniao")));

        Assert.Equal(33.33m, Reembolsavel(comOutro, "d-cafe"));
        Assert.Equal(56.67m, Reembolsavel(comOutro, "d-extra"));
    }

    private static ItemAvaliado Item(
        string id,
        DateOnly data,
        string categoria,
        decimal valor,
        string fornecedor,
        bool temNota = true)
    {
        var despesa = new Despesa(id, data, categoria, fornecedor, valor, temNota, "Despesa de teste");
        return ItemAvaliado.De(despesa);
    }

    private static ContextoAvaliacao Contexto(params ItemAvaliado[] itens) =>
        new(new Competencia(2026, 7), false, itens);

    private static ContextoAvaliacao Aplicar(ContextoAvaliacao contexto)
    {
        contexto = new Duplicatas().Aplicar(contexto);
        contexto = new Viagem().Aplicar(contexto);
        return new LimiteDiario().Aplicar(contexto);
    }

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
        contexto = new Viagem().Aplicar(contexto);

        return new LimiteDiario().Aplicar(contexto);
    }

    private static string[] IdsExceto(string id) =>
        Enumerable.Range(1, 14).Select(n => $"d-{n:000}").Where(x => x != id).ToArray();

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
