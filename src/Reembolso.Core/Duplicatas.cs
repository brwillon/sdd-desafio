namespace Reembolso.Core;

public sealed class Duplicatas
{
    public ContextoAvaliacao Aplicar(ContextoAvaliacao contexto)
    {
        var grupos = contexto.Itens
            .Where(i => i.Situacao == Situacao.Ativo)
            .GroupBy(i => (i.Despesa.Data, i.Categoria, i.Despesa.Fornecedor, i.Despesa.Valor))
            .Where(g => g.Count() > 1)
            .SelectMany(g =>
            {
                var alerta = $"Possível duplicata: {string.Join(", ", g.Select(i => i.Despesa.Id))}";
                return g.Select(i => (i.Despesa.Id, Alerta: alerta));
            })
            .ToDictionary(x => x.Id, x => x.Alerta);

        return contexto with { Itens = contexto.Itens.Select(i => Avaliar(i, grupos)).ToList() };
    }

    private static ItemAvaliado Avaliar(ItemAvaliado item, Dictionary<string, string> alertas)
    {
        if (!alertas.TryGetValue(item.Despesa.Id, out var alerta))
            return item;

        return item with
        {
            Situacao = Situacao.EmRevisao,
            ValorReembolsavel = 0m,
            Regras = item.Regras.Add("RN-005"),
            Alertas = item.Alertas.Add(alerta)
        };
    }
}
