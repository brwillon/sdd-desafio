using Reembolso.Cli;
using Reembolso.Core;

namespace Reembolso.Tests;

public class NotaFiscalObrigatoriaTests
{
    [Fact]
    public void RN004_D003Exato100SemNota_NaoExigeNota()
    {
        var resultado = AplicarAoExemplo().Single(i => i.Despesa.Id == "d-003");

        Assert.Equal(100m, resultado.Despesa.Valor);
        Assert.False(resultado.Despesa.TemNotaFiscal);
        Assert.Equal(Situacao.Ativo, resultado.Situacao);
        Assert.DoesNotContain("RN-004", resultado.Regras);
    }

    [Fact]
    public void RN004_D004_10001SemNota_Recusado()
    {
        var resultado = AplicarAoExemplo().Single(i => i.Despesa.Id == "d-004");

        Assert.Equal(100.01m, resultado.Despesa.Valor);
        Assert.Equal(Situacao.Recusado, resultado.Situacao);
        Assert.Equal(0m, resultado.ValorReembolsavel);
        Assert.Contains("RN-004", resultado.Regras);
    }

    [Fact]
    public void RN004_D013_690SemNota_Recusado()
    {
        var resultado = AplicarAoExemplo().Single(i => i.Despesa.Id == "d-013");

        Assert.Equal(690m, resultado.Despesa.Valor);
        Assert.Equal(Situacao.Recusado, resultado.Situacao);
        Assert.Equal(0m, resultado.ValorReembolsavel);
        Assert.Contains("RN-004", resultado.Regras);
    }

    [Fact]
    public void RN004_AcimaDe100ComNota_NaoEhAfetada()
    {
        var resultado = Aplicar(Despesa("d-x", "hospedagem", 480m, temNota: true));

        Assert.Equal(Situacao.Ativo, resultado.Single().Situacao);
    }

    [Fact]
    public void AMB005_NotaVerificadaPeloValorIndividualAntesDoCorte()
    {
        // Dois itens de 60,00 no mesmo dia somam 120,00, mas cada um está abaixo do limiar.
        var soma = Aplicar(
            Despesa("d-a", "alimentacao", 60m, temNota: false),
            Despesa("d-b", "alimentacao", 60m, temNota: false));

        Assert.All(soma, i => Assert.Equal(Situacao.Ativo, i.Situacao));

        // 150,00 sem nota é recusado, embora o limite diário de 60,00 pagasse menos que 100,00.
        var individual = Aplicar(Despesa("d-c", "alimentacao", 150m, temNota: false)).Single();

        Assert.Equal(Situacao.Recusado, individual.Situacao);
        Assert.Equal(0m, individual.ValorReembolsavel);
    }

    [Theory]
    [InlineData(100.00, Situacao.Ativo)]
    [InlineData(100.01, Situacao.Recusado)]
    [InlineData(99.99, Situacao.Ativo)]
    public void AMB009_AcimaDe100EhEstrito(double valor, Situacao esperado)
    {
        var resultado = Aplicar(Despesa("d-x", "alimentacao", (decimal)valor, temNota: false)).Single();

        Assert.Equal(esperado, resultado.Situacao);
    }

    [Fact]
    public void AMB021_SemNota_RecusaTotalSemPagamentoParcial()
    {
        // O limite diário de alimentação (60,00) pagaria algo, mas sem nota o item é zerado.
        var resultado = Aplicar(Despesa("d-x", "alimentacao", 150m, temNota: false)).Single();

        Assert.Equal(Situacao.Recusado, resultado.Situacao);
        Assert.Equal(0m, resultado.ValorReembolsavel);
        Assert.Contains("RN-004", resultado.Regras);
    }

    [Fact]
    public void RN004_LimiarNaoMudaEmViagem()
    {
        var resultado = Aplicar(new[] { Despesa("d-x", "alimentacao", 100.01m, temNota: false) }, viagem: true).Single();

        Assert.Equal(Situacao.Recusado, resultado.Situacao);
    }

    [Fact]
    public void RN004_ItemJaEmRevisao_NaoEhAlterado()
    {
        var despesa = Despesa("d-x", "alimentacao", 150m, temNota: false);
        var item = ItemAvaliado.De(despesa) with { Situacao = Situacao.EmRevisao, Regras = ["RN-003"] };
        var contexto = new ContextoAvaliacao(new Competencia(2026, 7), false, [item]);

        var resultado = new NotaFiscalObrigatoria().Aplicar(contexto).Itens.Single();

        Assert.Equal(Situacao.EmRevisao, resultado.Situacao);
        Assert.DoesNotContain("RN-004", resultado.Regras);
    }

    private static Despesa Despesa(string id, string categoria, decimal valor, bool temNota) =>
        new(id, new DateOnly(2026, 7, 10), categoria, "Fornecedor", valor, temNota, null);

    private static IReadOnlyList<ItemAvaliado> Aplicar(params Despesa[] despesas) =>
        Aplicar(despesas, viagem: false);

    private static IReadOnlyList<ItemAvaliado> Aplicar(IEnumerable<Despesa> despesas, bool viagem)
    {
        var contexto = new ContextoAvaliacao(
            new Competencia(2026, 7), viagem, despesas.Select(ItemAvaliado.De).ToList());

        return new NotaFiscalObrigatoria().Aplicar(contexto).Itens;
    }

    private static IReadOnlyList<ItemAvaliado> AplicarAoExemplo()
    {
        var entrada = LeitorEntrada.Ler(CaminhoDoExemplo());
        var contexto = new ContextoAvaliacao(
            entrada.Competencia, false, entrada.Despesas.Select(ItemAvaliado.De).ToList());

        return new NotaFiscalObrigatoria().Aplicar(contexto).Itens;
    }

    private static string CaminhoDoExemplo()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Reembolso.sln")))
            dir = dir.Parent;
        return Path.Combine(dir!.FullName, "exemplos", "despesas-exemplo.json");
    }
}
