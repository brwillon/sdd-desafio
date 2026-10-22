using System.Globalization;
using System.Text;

namespace Reembolso.Core;

public sealed class CategoriaNormalizada
{
    private static readonly HashSet<string> Aceitas =
        ["alimentacao", "transporte_urbano", "hospedagem"];

    public ContextoAvaliacao Aplicar(ContextoAvaliacao contexto) =>
        contexto with { Itens = contexto.Itens.Select(Avaliar).ToList() };

    private static ItemAvaliado Avaliar(ItemAvaliado item)
    {
        if (item.Situacao != Situacao.Ativo)
            return item;

        var normalizada = Normalizar(item.Despesa.Categoria);

        return Aceitas.Contains(normalizada)
            ? item with { Categoria = normalizada, Regras = item.Regras.Add("RN-001") }
            : item with
            {
                Situacao = Situacao.Recusado,
                ValorReembolsavel = 0m,
                Regras = item.Regras.Add("RN-001"),
                Alertas = item.Alertas.Add("Categoria não reconhecida: encaminhar para análise humana.")
            };
    }

    private static string Normalizar(string categoria)
    {
        var decomposta = categoria.Trim().Normalize(NormalizationForm.FormD);
        var semAcento = new StringBuilder();
        foreach (var c in decomposta)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                continue;
            semAcento.Append(c is ' ' or '-' ? '_' : char.ToLowerInvariant(c));
        }
        return semAcento.ToString();
    }
}
