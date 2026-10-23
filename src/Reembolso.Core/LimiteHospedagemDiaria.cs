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

        return item with
        {
            Situacao = Situacao.Aprovado,
            ValorReembolsavel = Math.Min(item.Despesa.Valor, LimitePorDiaria)
        };
    }
}
