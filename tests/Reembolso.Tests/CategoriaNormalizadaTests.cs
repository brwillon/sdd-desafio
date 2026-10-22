using Reembolso.Cli;
using Reembolso.Core;

namespace Reembolso.Tests;

public class CategoriaNormalizadaTests
{
    [Fact]
    public void RN001_D014Maiuscula_NormalizadaParaAlimentacao()
    {
        var contexto = ContextoDoExemplo();

        var resultado = new CategoriaNormalizada().Aplicar(contexto);

        var d014 = resultado.Itens.Single(i => i.Despesa.Id == "d-014");
        Assert.Equal("alimentacao", d014.Categoria);
        Assert.Equal(Situacao.Ativo, d014.Situacao);
        Assert.Contains("RN-001", d014.Regras);
    }

    [Theory]
    [InlineData("  Alimentação  ", "alimentacao")]
    [InlineData("TRANSPORTE_URBANO", "transporte_urbano")]
    [InlineData("\tHospedagém\n", "hospedagem")]
    public void AMB013_AcentoCaixaEEspacosNasPontas_Normalizados(string informada, string esperada)
    {
        var resultado = Aplicar(informada);

        Assert.Equal(esperada, resultado.Categoria);
        Assert.Equal(Situacao.Ativo, resultado.Situacao);
    }

    [Theory]
    [InlineData("transporte urbano")]
    [InlineData("transporte-urbano")]
    [InlineData("Transporte-Urbano")]
    public void AMB013_EspacoOuHifenEntrePalavras_EquivaleASublinhado(string informada)
    {
        var resultado = Aplicar(informada);

        Assert.Equal("transporte_urbano", resultado.Categoria);
        Assert.Equal(Situacao.Ativo, resultado.Situacao);
    }

    [Fact]
    public void AMB018_D005Coworking_RecusadoComZero()
    {
        var contexto = ContextoDoExemplo();

        var resultado = new CategoriaNormalizada().Aplicar(contexto);

        var d005 = resultado.Itens.Single(i => i.Despesa.Id == "d-005");
        Assert.Equal(Situacao.Recusado, d005.Situacao);
        Assert.Equal(0m, d005.ValorReembolsavel);
        Assert.Contains("RN-001", d005.Regras);
    }

    [Fact]
    public void RN001_CategoriaNaoReconhecida_RecusadaComAlertaDeAnaliseHumana()
    {
        var resultado = Aplicar("passagem aerea");

        Assert.Equal(Situacao.Recusado, resultado.Situacao);
        Assert.Equal(0m, resultado.ValorReembolsavel);
        Assert.Equal("passagem aerea", resultado.Categoria);
        Assert.Contains("RN-001", resultado.Regras);
        Assert.Contains(resultado.Alertas, a => a.Contains("análise humana", StringComparison.OrdinalIgnoreCase));
    }

    private static ItemAvaliado Aplicar(string categoria)
    {
        var despesa = new Despesa("d-x", new DateOnly(2026, 7, 10), categoria, "Fornecedor", 50m, true, null);
        var contexto = new ContextoAvaliacao(
            new Competencia(2026, 7), false, [ItemAvaliado.De(despesa)]);

        return new CategoriaNormalizada().Aplicar(contexto).Itens.Single();
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
