namespace Reembolso.Core;

public sealed class ValorNegativo
{
    public ContextoAvaliacao Aplicar(ContextoAvaliacao contexto) =>
        contexto with { Itens = contexto.Itens.Select(Avaliar).ToList() };

    private static ItemAvaliado Avaliar(ItemAvaliado item) =>
        item.Situacao == Situacao.Ativo && item.Despesa.Valor < 0m
            ? item with
            {
                Situacao = Situacao.Recusado,
                ValorReembolsavel = 0m,
                Justificativa = "Valor negativo é dado inválido.",
                Regras = item.Regras.Add("RN-002")
            }
            : item;
}
