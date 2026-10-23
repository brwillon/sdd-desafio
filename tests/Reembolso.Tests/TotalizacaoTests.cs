using System.Text.RegularExpressions;
using Reembolso.Cli;
using Reembolso.Core;

namespace Reembolso.Tests;

public class TotalizacaoTests
{
    [Fact]
    public void RN011_ArquivoCompleto_TotalReembolsavelEh58153()
    {
        var resultado = ExecutarExemplo();

        Assert.Equal(581.53m, resultado.TotalReembolsavel);
        Assert.True(resultado.Viagem);
    }

    [Fact]
    public void RN011_ArquivoCompleto_Resumo7Aprovado4Recusado3EmRevisao()
    {
        var resultado = ExecutarExemplo();

        Assert.Equal(7, resultado.Resumo.Aprovado);
        Assert.Equal(4, resultado.Resumo.Recusado);
        Assert.Equal(3, resultado.Resumo.EmRevisao);
    }

    [Fact]
    public void RN011_ArquivoCompleto_14ItensNaOrdemDaEntrada()
    {
        var resultado = ExecutarExemplo();

        Assert.Equal(14, resultado.Itens.Count);
        Assert.Equal(
            Enumerable.Range(1, 14).Select(n => $"d-{n:000}"),
            resultado.Itens.Select(i => i.Despesa.Id));
    }

    [Fact]
    public void RN011_TodoItemTemJustificativaEReferenciaRNExistente()
    {
        var resultado = ExecutarExemplo();
        var regrasDaSpec = RegrasDefinidasNaSpec();

        Assert.NotEmpty(regrasDaSpec);
        foreach (var item in resultado.Itens)
        {
            Assert.False(
                string.IsNullOrWhiteSpace(item.Justificativa),
                $"{item.Despesa.Id} sem justificativa");
            Assert.NotEmpty(item.Regras);
            Assert.All(item.Regras, regra => Assert.Contains(regra, regrasDaSpec));
        }
    }

    [Fact]
    public void RN010_RecusadoEEmRevisao_ValorReembolsavelEhZero()
    {
        var resultado = ExecutarExemplo();

        var semPagamento = resultado.Itens
            .Where(i => i.Situacao is Situacao.Recusado or Situacao.EmRevisao)
            .ToList();

        Assert.Equal(
            new[] { "d-004", "d-005", "d-006", "d-007", "d-008", "d-009", "d-013" },
            semPagamento.Select(i => i.Despesa.Id));
        Assert.All(semPagamento, i => Assert.Equal(0.00m, i.ValorReembolsavel));
    }

    [Fact]
    public void RN010_EmRevisao_NaoEntraNoTotal()
    {
        var resultado = ExecutarExemplo();

        var emRevisao = resultado.Itens.Where(i => i.Situacao == Situacao.EmRevisao).ToList();
        Assert.Equal(new[] { "d-006", "d-007", "d-008" }, emRevisao.Select(i => i.Despesa.Id));

        // Se os três entrassem pelo valor pedido (54,90 + 54,90 + 41,00), o total passaria de 581,53.
        var somaDosAprovados = resultado.Itens
            .Where(i => i.Situacao == Situacao.Aprovado)
            .Sum(i => i.ValorReembolsavel);
        Assert.Equal(somaDosAprovados, resultado.TotalReembolsavel);
        Assert.Equal(581.53m, resultado.TotalReembolsavel);
    }

    [Fact]
    public void SEC4_SubconjuntoD001D002D004D010_BateComSaidaDaSpec()
    {
        var ids = new[] { "d-001", "d-002", "d-004", "d-010" };
        var entrada = LeitorEntrada.Ler(CaminhoDoExemplo());
        var despesas = entrada.Despesas.Where(d => ids.Contains(d.Id)).ToList();

        var resultado = Executar(entrada.ColaboradorId, entrada.Competencia, despesas);

        Assert.Equal("c-0417", resultado.ColaboradorId);
        Assert.Equal(new Competencia(2026, 7), resultado.Competencia);
        Assert.True(resultado.Viagem);
        Assert.Equal(340.00m, resultado.TotalReembolsavel);
        Assert.Equal(3, resultado.Resumo.Aprovado);
        Assert.Equal(1, resultado.Resumo.Recusado);
        Assert.Equal(0, resultado.Resumo.EmRevisao);
        Assert.Equal(ids, resultado.Itens.Select(i => i.Despesa.Id));

        ConferirItem(resultado, "d-001", "alimentacao", Situacao.Aprovado, 72.50m, 72.50m,
            "Dentro do saldo diário de alimentação (limite 90,00 em viagem).",
            new[] { "RN-001", "RN-006", "RN-007" });
        ConferirItem(resultado, "d-002", "alimentacao", Situacao.Aprovado, 38.00m, 17.50m,
            "Restava 17,50 do saldo diário de 90,00; excedente cortado.",
            new[] { "RN-001", "RN-006", "RN-007" });
        ConferirItem(resultado, "d-004", "transporte_urbano", Situacao.Recusado, 100.01m, 0.00m,
            "Valor acima de 100,00 sem nota fiscal.",
            new[] { "RN-001", "RN-004" });
        ConferirItem(resultado, "d-010", "hospedagem", Situacao.Aprovado, 480.00m, 250.00m,
            "Lançamento conta como uma diária; limite de 250,00; excedente cortado.",
            new[] { "RN-001", "RN-008" });
    }

    private static void ConferirItem(
        Resultado resultado,
        string id,
        string categoria,
        Situacao situacao,
        decimal valorSolicitado,
        decimal valorReembolsavel,
        string justificativa,
        string[] regras)
    {
        var item = resultado.Itens.Single(i => i.Despesa.Id == id);

        Assert.Equal(categoria, item.Categoria);
        Assert.Equal(situacao, item.Situacao);
        Assert.Equal(valorSolicitado, item.Despesa.Valor);
        Assert.Equal(valorReembolsavel, item.ValorReembolsavel);
        Assert.Equal(justificativa, item.Justificativa);
        Assert.Equal(regras, item.Regras);
        Assert.Empty(item.Alertas);
    }

    private static Resultado ExecutarExemplo()
    {
        var entrada = LeitorEntrada.Ler(CaminhoDoExemplo());
        return Executar(entrada.ColaboradorId, entrada.Competencia, entrada.Despesas);
    }

    // Passos 1 a 8 da seção 8 da spec, em ordem; o passo 9 é o que está sob teste.
    private static Resultado Executar(
        string colaboradorId,
        Competencia competencia,
        IEnumerable<Despesa> despesas)
    {
        var itens = despesas.Select(ItemAvaliado.De).ToList();
        var contexto = new ContextoAvaliacao(competencia, false, itens);

        contexto = new ValorNegativo().Aplicar(contexto);
        contexto = new CategoriaNormalizada().Aplicar(contexto);
        contexto = new PeriodoCompetencia().Aplicar(contexto);
        contexto = new NotaFiscalObrigatoria().Aplicar(contexto);
        contexto = new Duplicatas().Aplicar(contexto);
        contexto = new Viagem().Aplicar(contexto);
        contexto = new LimiteDiario().Aplicar(contexto);
        contexto = new LimiteHospedagemDiaria().Aplicar(contexto);

        return new Totalizacao().Aplicar(contexto, colaboradorId);
    }

    private static IReadOnlySet<string> RegrasDefinidasNaSpec()
    {
        var spec = File.ReadAllText(
            Path.Combine(RaizDaSolucao(), "specs", "001-motor-reembolso", "spec.md"));

        return Regex.Matches(spec, @"^### (RN-\d{3})\b", RegexOptions.Multiline)
            .Select(m => m.Groups[1].Value)
            .ToHashSet();
    }

    private static string CaminhoDoExemplo() =>
        Path.Combine(RaizDaSolucao(), "exemplos", "despesas-exemplo.json");

    private static string RaizDaSolucao()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Reembolso.sln")))
            dir = dir.Parent;
        return dir!.FullName;
    }
}
