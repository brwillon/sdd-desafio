using Reembolso.Cli;
using Reembolso.Core;

namespace Reembolso.Tests;

public class ArredondamentoTests
{
    [Fact]
    public void RN009_33333_Vira3333()
    {
        Assert.Equal(33.33m, Arredondamento.Aplicar(33.333m));
    }

    [Fact]
    public void RN009_Empate10125_Vira1012()
    {
        Assert.Equal(10.12m, Arredondamento.Aplicar(10.125m));
    }

    [Fact]
    public void RN009_Empate10135_Vira1014()
    {
        Assert.Equal(10.14m, Arredondamento.Aplicar(10.135m));
    }

    [Fact]
    public void AMB012_D011_ValorSolicitadoEh3333()
    {
        var entrada = LeitorEntrada.Ler(CaminhoDoExemplo());
        var d011 = entrada.Despesas.Single(d => d.Id == "d-011");

        Assert.Equal(33.33m, Arredondamento.Aplicar(d011.Valor));
    }

    private static string CaminhoDoExemplo()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Reembolso.sln")))
            dir = dir.Parent;
        return Path.Combine(dir!.FullName, "exemplos", "despesas-exemplo.json");
    }
}
