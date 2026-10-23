namespace Reembolso.Core;

public sealed class LimiteHospedagemDiaria
{
    private const decimal LimitePorDiaria = 250m;

    public ContextoAvaliacao Aplicar(ContextoAvaliacao contexto)
    {
        var itens = contexto.Itens.Select(Avaliar).ToList();

        return contexto with { Itens = itens };
    }

    private static ItemAvaliado Avaliar(ItemAvaliado item)
    {
        if (item.Situacao != Situacao.Ativo || item.Categoria != "hospedagem")
            return item;

        var pago = Math.Min(item.Despesa.Valor, LimitePorDiaria);
        var justificativa = pago < item.Despesa.Valor
            ? $"Lançamento conta como uma diária; limite de {Moeda.Formatar(LimitePorDiaria)}; excedente cortado."
            : $"Lançamento conta como uma diária; dentro do limite de {Moeda.Formatar(LimitePorDiaria)}.";

        return item with
        {
            Situacao = Situacao.Aprovado,
            ValorReembolsavel = pago,
            Justificativa = justificativa,
            Regras = item.Regras.Add("RN-008")
        };
    }
}
