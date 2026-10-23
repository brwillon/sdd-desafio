using System.Text;
using System.Text.Json;
using Reembolso.Core;

namespace Reembolso.Cli;

public static class Programa
{
    public static int Executar(string[] args)
    {
        if (args.Length != 5 || args[0] != "calcular" || args[1] != "--input" || args[3] != "--output")
        {
            Console.Error.WriteLine("Uso: calcular --input <arquivo.json> --output <arquivo.json>");
            return 2;
        }

        try
        {
            var entrada = LeitorEntrada.Ler(args[2]);
            var resultado = Calcular(entrada);
            File.WriteAllText(args[4], SaidaJson.Serializar(resultado), new UTF8Encoding(false));
            return 0;
        }
        catch (JsonException e)
        {
            Console.Error.WriteLine($"Entrada ilegível: {e.Message}");
            return 1;
        }
    }

    // Passos 1 a 9 da seção 8 da spec, em ordem.
    private static Resultado Calcular(EntradaDespesas entrada)
    {
        var itens = entrada.Despesas.Select(ItemAvaliado.De).ToList();
        var contexto = new ContextoAvaliacao(entrada.Competencia, false, itens);

        contexto = new ValorNegativo().Aplicar(contexto);
        contexto = new CategoriaNormalizada().Aplicar(contexto);
        contexto = new PeriodoCompetencia().Aplicar(contexto);
        contexto = new NotaFiscalObrigatoria().Aplicar(contexto);
        contexto = new Duplicatas().Aplicar(contexto);
        contexto = new Viagem().Aplicar(contexto);
        contexto = new LimiteDiario().Aplicar(contexto);
        contexto = new LimiteHospedagemDiaria().Aplicar(contexto);

        return new Totalizacao().Aplicar(contexto, entrada.ColaboradorId);
    }
}
