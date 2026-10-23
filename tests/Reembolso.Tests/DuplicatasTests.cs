using Reembolso.Cli;
using Reembolso.Core;

namespace Reembolso.Tests;

public class DuplicatasTests
{
    [Fact]
    public void RN005_D006D007_EmRevisaoComAlertaCitandoOsDoisIds()
    {
        var resultado = AplicarAoExemplo();

        foreach (var id in new[] { "d-006", "d-007" })
        {
            var item = resultado.Single(i => i.Despesa.Id == id);

            Assert.Equal(Situacao.EmRevisao, item.Situacao);
            Assert.Contains("RN-005", item.Regras);
            AssertAlertaCita(item, "d-006", "d-007");
        }
    }

    [Fact]
    public void RN005_D001D002FornecedoresDiferentes_NaoFormamGrupo()
    {
        var resultado = AplicarAoExemplo();

        foreach (var id in new[] { "d-001", "d-002" })
        {
            var item = resultado.Single(i => i.Despesa.Id == id);

            Assert.Equal(Situacao.Ativo, item.Situacao);
            Assert.DoesNotContain("RN-005", item.Regras);
            Assert.Empty(item.Alertas);
        }
    }

    [Fact]
    public void RN005_SoOFornecedorDiferente_NaoFormaGrupo()
    {
        // d-001 e d-002 também diferem no valor; aqui apenas o fornecedor muda.
        var resultado = Aplicar(Item("d-a", fornecedor: "Bistro Central"), Item("d-b", fornecedor: "Cantina do Porto"));

        Assert.All(resultado, i =>
        {
            Assert.Equal(Situacao.Ativo, i.Situacao);
            Assert.DoesNotContain("RN-005", i.Regras);
        });
    }

    [Fact]
    public void RN005_D003D004ValoresDiferentes_NaoFormamGrupo()
    {
        // d-003 (100,00) e d-004 (100,01) têm mesma data, categoria e fornecedor, mas valores distintos.
        var d003 = Item("d-003", valor: 100m, temNota: false);
        var d004 = Item("d-004", valor: 100.01m, temNota: false);

        var resultado = Aplicar(d003, d004);

        Assert.All(resultado, i =>
        {
            Assert.Equal(Situacao.Ativo, i.Situacao);
            Assert.DoesNotContain("RN-005", i.Regras);
            Assert.Empty(i.Alertas);
        });
    }

    [Fact]
    public void RN005_GrupoDeTres_TodosEmRevisaoComAlertaCitandoOsTres()
    {
        var resultado = Aplicar(Item("d-a"), Item("d-b"), Item("d-c"));

        Assert.Equal(3, resultado.Count);
        Assert.All(resultado, i =>
        {
            Assert.Equal(Situacao.EmRevisao, i.Situacao);
            Assert.Contains("RN-005", i.Regras);
            AssertAlertaCita(i, "d-a", "d-b", "d-c");
        });
    }

    [Fact]
    public void RN010_DuplicataEmRevisao_ZeraValorReembolsavel()
    {
        // O consumo de saldo diário é do passo 8 (T-009). Aqui: valor pré-existente é zerado.
        var a = Item("d-a") with { ValorReembolsavel = 54.90m };
        var b = Item("d-b") with { ValorReembolsavel = 54.90m };

        var resultado = Aplicar(a, b);

        Assert.All(resultado, i =>
        {
            Assert.Equal(Situacao.EmRevisao, i.Situacao);
            Assert.Equal(0m, i.ValorReembolsavel);
        });
    }

    [Theory]
    [InlineData(Situacao.Recusado)]
    [InlineData(Situacao.EmRevisao)]
    public void RN005_ItemNaoAtivo_NaoParticipaDoGrupo(Situacao situacao)
    {
        var inativo = Item("d-a") with { Situacao = situacao, Regras = ["RN-003"] };
        var ativo = Item("d-b");

        var resultado = Aplicar(inativo, ativo);

        var depoisInativo = resultado.Single(i => i.Despesa.Id == "d-a");
        var depoisAtivo = resultado.Single(i => i.Despesa.Id == "d-b");
        Assert.Equal(situacao, depoisInativo.Situacao);
        Assert.DoesNotContain("RN-005", depoisInativo.Regras);
        Assert.Equal(Situacao.Ativo, depoisAtivo.Situacao);
        Assert.DoesNotContain("RN-005", depoisAtivo.Regras);
        Assert.Empty(depoisAtivo.Alertas);
    }

    [Fact]
    public void AMB007_CriterioEhDataCategoriaFornecedorEValor()
    {
        var baseItem = Item("d-base");

        // Cada campo do critério, sozinho, desfaz o grupo.
        var diferentes = new[]
        {
            Item("d-data", data: new DateOnly(2026, 7, 11)),
            Item("d-categoria", categoria: "transporte_urbano"),
            Item("d-fornecedor", fornecedor: "Outro Fornecedor"),
            Item("d-valor", valor: 55m),
        };

        foreach (var diferente in diferentes)
        {
            var resultado = Aplicar(baseItem, diferente);

            Assert.All(resultado, i => Assert.Equal(Situacao.Ativo, i.Situacao));
        }

        // Campos fora do critério (descrição, nota fiscal) não impedem o grupo.
        var outraDescricao = Item("d-desc", descricao: "Jantar", temNota: false);
        var agrupados = Aplicar(baseItem, outraDescricao);

        Assert.All(agrupados, i => Assert.Equal(Situacao.EmRevisao, i.Situacao));
    }

    [Fact]
    public void SEC8_IguaisSemNotaAcima100_AmbasRecusadasPorRN004()
    {
        var itens = new[]
        {
            Item("d-a", valor: 150m, temNota: false),
            Item("d-b", valor: 150m, temNota: false),
        };
        var contexto = new ContextoAvaliacao(new Competencia(2026, 7), false, itens);

        var aposNota = new NotaFiscalObrigatoria().Aplicar(contexto);
        var resultado = new Duplicatas().Aplicar(aposNota).Itens;

        Assert.All(resultado, i =>
        {
            Assert.Equal(Situacao.Recusado, i.Situacao);
            Assert.Contains("RN-004", i.Regras);
            Assert.DoesNotContain("RN-005", i.Regras);
            Assert.Empty(i.Alertas);
        });
    }

    private static void AssertAlertaCita(ItemAvaliado item, params string[] ids)
    {
        Assert.Contains("RN-005", item.Regras);
        Assert.Contains(item.Alertas, alerta => ids.All(alerta.Contains));
    }

    private static ItemAvaliado Item(
        string id,
        DateOnly? data = null,
        string categoria = "alimentacao",
        string fornecedor = "Bistro Central",
        decimal valor = 54.90m,
        bool temNota = true,
        string? descricao = "Almoco")
    {
        var despesa = new Despesa(id, data ?? new DateOnly(2026, 7, 9), categoria, fornecedor, valor, temNota, descricao);
        return ItemAvaliado.De(despesa);
    }

    private static IReadOnlyList<ItemAvaliado> Aplicar(params ItemAvaliado[] itens) =>
        new Duplicatas().Aplicar(new ContextoAvaliacao(new Competencia(2026, 7), false, itens)).Itens;

    private static IReadOnlyList<ItemAvaliado> AplicarAoExemplo()
    {
        var entrada = LeitorEntrada.Ler(CaminhoDoExemplo());
        var contexto = new ContextoAvaliacao(
            entrada.Competencia, false, entrada.Despesas.Select(ItemAvaliado.De).ToList());

        return new Duplicatas().Aplicar(contexto).Itens;
    }

    private static string CaminhoDoExemplo()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Reembolso.sln")))
            dir = dir.Parent;
        return Path.Combine(dir!.FullName, "exemplos", "despesas-exemplo.json");
    }
}
