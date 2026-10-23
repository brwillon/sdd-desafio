namespace Reembolso.Core;

public sealed class Totalizacao
{
    public Resultado Aplicar(ContextoAvaliacao contexto, string colaboradorId)
    {
        var total = contexto.Itens
            .Where(i => i.Situacao == Situacao.Aprovado)
            .Sum(i => i.ValorReembolsavel);

        var resumo = new ResumoStatus(
            contexto.Itens.Count(i => i.Situacao == Situacao.Aprovado),
            contexto.Itens.Count(i => i.Situacao == Situacao.Recusado),
            contexto.Itens.Count(i => i.Situacao == Situacao.EmRevisao));

        return new Resultado(
            colaboradorId,
            contexto.Competencia,
            contexto.Viagem,
            total,
            resumo,
            contexto.Itens);
    }
}
