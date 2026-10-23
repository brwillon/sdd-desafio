namespace Reembolso.Core;

public sealed class NotaFiscalObrigatoria
{
    private const decimal LimiarSemNota = 100m;

    public ContextoAvaliacao Aplicar(ContextoAvaliacao contexto) =>
        contexto with { Itens = contexto.Itens.Select(Avaliar).ToList() };

    private static ItemAvaliado Avaliar(ItemAvaliado item)
    {
        if (item.Situacao != Situacao.Ativo)
            return item;

        if (item.Despesa.TemNotaFiscal || item.Despesa.Valor <= LimiarSemNota)
            return item;

        return item with
        {
            Situacao = Situacao.Recusado,
            ValorReembolsavel = 0m,
            Regras = item.Regras.Add("RN-004")
        };
    }
}
