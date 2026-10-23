using System.Reflection;
using System.Text.RegularExpressions;

namespace Reembolso.Tests;

public class RastreabilidadeTests
{
    [Fact]
    public void TRACE_TodaRNeAMBDaSpecApareceEmTasks()
    {
        var spec = File.ReadAllText(CaminhoDoRepositorio("specs", "001-motor-reembolso", "spec.md"));
        var idsDaSpec = Regex.Matches(spec, @"\b(?:RN|AMB)-\d{3}\b")
            .Select(m => m.Value)
            .ToHashSet();

        var idsDaTabela = LinhasDaTabelaCobertura()
            .Select(colunas => colunas[0])
            .ToHashSet();

        Assert.NotEmpty(idsDaSpec);
        var ausentes = idsDaSpec.Except(idsDaTabela).Order().ToList();
        Assert.True(ausentes.Count == 0,
            $"IDs da spec sem linha na tabela Cobertura de tasks.md: {string.Join(", ", ausentes)}");
    }

    [Fact]
    public void TRACE_CoberturaCitaApenasTestesExistentes()
    {
        var existentes = typeof(RastreabilidadeTests).Assembly.GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            .Where(m => m.GetCustomAttributes<FactAttribute>(inherit: true).Any())
            .Select(m => m.Name)
            .ToHashSet();

        var citados = LinhasDaTabelaCobertura()
            .SelectMany(colunas => Regex.Matches(colunas[2], @"`([^`]+)`").Select(m => m.Groups[1].Value))
            .Where(nome => Regex.IsMatch(nome, @"^[A-Z]+\d*_\w+$"))
            .ToList();

        Assert.NotEmpty(citados);
        var inexistentes = citados.Where(nome => !existentes.Contains(nome)).Distinct().Order().ToList();
        Assert.True(inexistentes.Count == 0,
            $"Testes citados em Cobertura que não existem no assembly: {string.Join(", ", inexistentes)}");
    }

    private static IEnumerable<string[]> LinhasDaTabelaCobertura()
    {
        var tasks = File.ReadAllLines(CaminhoDoRepositorio("specs", "001-motor-reembolso", "tasks.md"));
        var inicio = Array.FindIndex(tasks, l => l.TrimEnd() == "## Cobertura");
        Assert.True(inicio >= 0, "Seção '## Cobertura' não encontrada em tasks.md");

        return tasks.Skip(inicio + 1)
            .Where(l => l.StartsWith('|'))
            .Select(l => l.Trim().Trim('|').Split('|').Select(c => c.Trim()).ToArray())
            .Where(colunas => colunas.Length == 3 && !colunas[0].StartsWith("---") && colunas[0] != "Regra da spec");
    }

    private static string CaminhoDoRepositorio(params string[] partes)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Reembolso.sln")))
            dir = dir.Parent;
        return Path.Combine([dir!.FullName, .. partes]);
    }
}
