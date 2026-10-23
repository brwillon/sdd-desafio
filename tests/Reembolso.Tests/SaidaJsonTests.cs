using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Reembolso.Cli;
using Reembolso.Core;

namespace Reembolso.Tests;

public class SaidaJsonTests
{
    private static readonly string[] PropriedadesDoResultado =
    {
        "colaborador_id", "competencia", "viagem", "total_reembolsavel", "resumo", "itens"
    };

    private static readonly string[] PropriedadesDoResumo = { "aprovado", "recusado", "em_revisao" };

    private static readonly string[] PropriedadesDoItem =
    {
        "id", "categoria", "status", "valor_solicitado", "valor_reembolsavel",
        "justificativa", "regras", "alertas"
    };

    [Fact]
    public void SEC4_SaidaJson_PropriedadesNaOrdemDaSecao4E2CasasDecimais()
    {
        var ids = new[] { "d-001", "d-002", "d-004", "d-010" };
        var entrada = LeitorEntrada.Ler(CaminhoDoExemplo());
        var despesas = entrada.Despesas.Where(d => ids.Contains(d.Id)).ToList();
        var resultado = Calcular(entrada.ColaboradorId, entrada.Competencia, despesas);

        var json = SaidaJson.Serializar(resultado);

        using var doc = JsonDocument.Parse(json);
        var raiz = doc.RootElement;
        Assert.Equal(PropriedadesDoResultado, Nomes(raiz));
        Assert.Equal(PropriedadesDoResumo, Nomes(raiz.GetProperty("resumo")));
        var itens = raiz.GetProperty("itens").EnumerateArray().ToList();
        Assert.Equal(ids, itens.Select(i => i.GetProperty("id").GetString()));
        Assert.All(itens, item => Assert.Equal(PropriedadesDoItem, Nomes(item)));

        Assert.Equal("c-0417", raiz.GetProperty("colaborador_id").GetString());
        Assert.Equal("2026-07", raiz.GetProperty("competencia").GetString());
        Assert.Equal("aprovado", itens[0].GetProperty("status").GetString());
        Assert.Equal("recusado", itens[2].GetProperty("status").GetString());

        // Números com exatamente 2 casas: 340.00 e 0.00 não podem sair como 340 e 0.
        Assert.Matches(new Regex(@"""total_reembolsavel""\s*:\s*340\.00(?![\d])"), json);
        Assert.Matches(new Regex(@"""valor_solicitado""\s*:\s*72\.50(?![\d])"), json);
        Assert.Matches(new Regex(@"""valor_reembolsavel""\s*:\s*17\.50(?![\d])"), json);
        Assert.Matches(new Regex(@"""valor_solicitado""\s*:\s*100\.01(?![\d])"), json);
        Assert.Matches(new Regex(@"""valor_reembolsavel""\s*:\s*0\.00(?![\d])"), json);
        Assert.Matches(new Regex(@"""valor_solicitado""\s*:\s*480\.00(?![\d])"), json);
        Assert.Matches(new Regex(@"""valor_reembolsavel""\s*:\s*250\.00(?![\d])"), json);
    }

    [Fact]
    public void SEC9_ArquivoCompleto_ItensAprovadosERecusadosConformeCriterios()
    {
        var entrada = LeitorEntrada.Ler(CaminhoDoExemplo());
        var resultado = Calcular(entrada.ColaboradorId, entrada.Competencia, entrada.Despesas);

        using var doc = JsonDocument.Parse(SaidaJson.Serializar(resultado));
        var raiz = doc.RootElement;
        var itens = raiz.GetProperty("itens").EnumerateArray().ToList();

        Assert.Equal(581.53m, raiz.GetProperty("total_reembolsavel").GetDecimal());
        Assert.True(raiz.GetProperty("viagem").GetBoolean());
        Assert.Equal(14, itens.Count);
        Assert.Equal(
            Enumerable.Range(1, 14).Select(n => $"d-{n:000}"),
            itens.Select(i => i.GetProperty("id").GetString()));

        var resumo = raiz.GetProperty("resumo");
        Assert.Equal(7, resumo.GetProperty("aprovado").GetInt32());
        Assert.Equal(4, resumo.GetProperty("recusado").GetInt32());
        Assert.Equal(3, resumo.GetProperty("em_revisao").GetInt32());

        var aprovados = itens
            .Where(i => i.GetProperty("status").GetString() == "aprovado")
            .ToDictionary(i => i.GetProperty("id").GetString()!, i => i.GetProperty("valor_reembolsavel").GetDecimal());
        Assert.Equal(
            new Dictionary<string, decimal>
            {
                ["d-001"] = 72.50m, ["d-002"] = 17.50m, ["d-003"] = 100.00m, ["d-010"] = 250.00m,
                ["d-011"] = 33.33m, ["d-012"] = 47.20m, ["d-014"] = 61.00m
            },
            aprovados);

        Assert.Equal(new[] { "d-004", "d-005", "d-009", "d-013" }, IdsComStatus(itens, "recusado"));
        Assert.Equal(new[] { "d-006", "d-007", "d-008" }, IdsComStatus(itens, "em_revisao"));

        var semPagamento = itens.Where(i => i.GetProperty("status").GetString() != "aprovado");
        Assert.All(semPagamento, i => Assert.Equal(0.00m, i.GetProperty("valor_reembolsavel").GetDecimal()));
        Assert.All(itens, i => Assert.False(string.IsNullOrWhiteSpace(i.GetProperty("justificativa").GetString())));
    }

    [Fact]
    public void SEC9_DuasExecucoesComAMesmaEntrada_GeramBytesIdenticos()
    {
        var pasta = NovaPastaTemporaria();
        try
        {
            var saidaA = Path.Combine(pasta, "a.json");
            var saidaB = Path.Combine(pasta, "b.json");

            Assert.Equal(0, Programa.Executar(Argumentos(CaminhoDoExemplo(), saidaA)));
            Assert.Equal(0, Programa.Executar(Argumentos(CaminhoDoExemplo(), saidaB)));

            var bytesA = File.ReadAllBytes(saidaA);
            Assert.NotEmpty(bytesA);
            Assert.Equal(bytesA, File.ReadAllBytes(saidaB));
        }
        finally
        {
            Directory.Delete(pasta, recursive: true);
        }
    }

    [Fact]
    public void SEC9_CliCalcular_ExemploOficial_GeraArquivoDeSaida()
    {
        var pasta = NovaPastaTemporaria();
        try
        {
            var saida = Path.Combine(pasta, "resultado.json");

            var codigo = Programa.Executar(Argumentos(CaminhoDoExemplo(), saida));

            Assert.Equal(0, codigo);
            Assert.True(File.Exists(saida));
            using var doc = JsonDocument.Parse(File.ReadAllText(saida, Encoding.UTF8));
            var raiz = doc.RootElement;
            Assert.Equal(PropriedadesDoResultado, Nomes(raiz));
            Assert.Equal("c-0417", raiz.GetProperty("colaborador_id").GetString());
            Assert.Equal(581.53m, raiz.GetProperty("total_reembolsavel").GetDecimal());
            Assert.Equal(14, raiz.GetProperty("itens").GetArrayLength());
        }
        finally
        {
            Directory.Delete(pasta, recursive: true);
        }
    }

    private static string[] Argumentos(string entrada, string saida) =>
        new[] { "calcular", "--input", entrada, "--output", saida };

    private static IEnumerable<string> Nomes(JsonElement objeto) =>
        objeto.EnumerateObject().Select(p => p.Name);

    private static IEnumerable<string?> IdsComStatus(IEnumerable<JsonElement> itens, string status) =>
        itens.Where(i => i.GetProperty("status").GetString() == status)
            .Select(i => i.GetProperty("id").GetString());

    // Passos 1 a 9 da seção 8 da spec, em ordem.
    private static Resultado Calcular(
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

    private static string NovaPastaTemporaria()
    {
        var pasta = Path.Combine(Path.GetTempPath(), "reembolso-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(pasta);
        return pasta;
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
