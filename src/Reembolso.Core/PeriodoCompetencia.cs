namespace Reembolso.Core;

public sealed class PeriodoCompetencia
{
    public ContextoAvaliacao Aplicar(ContextoAvaliacao contexto) =>
        contexto with { Itens = contexto.Itens.Select(i => Avaliar(i, contexto.Competencia)).ToList() };

    private static ItemAvaliado Avaliar(ItemAvaliado item, Competencia competencia)
    {
        if (item.Situacao != Situacao.Ativo)
            return item;

        var data = item.Despesa.Data;
        if (data.Year == competencia.Ano && data.Month == competencia.Mes)
            return item;

        return item with
        {
            Situacao = Situacao.EmRevisao,
            ValorReembolsavel = 0m,
            Regras = item.Regras.Add("RN-003")
        };
    }
}
