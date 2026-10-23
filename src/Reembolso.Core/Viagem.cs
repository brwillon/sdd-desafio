namespace Reembolso.Core;

public sealed class Viagem
{
    public ContextoAvaliacao Aplicar(ContextoAvaliacao contexto) =>
        contexto with
        {
            Viagem = contexto.Itens.Any(i => i.Situacao == Situacao.Ativo && i.Categoria == "hospedagem")
        };
}
