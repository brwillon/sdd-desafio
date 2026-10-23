namespace Reembolso.Core;

public sealed class LimiteDiario
{
    private const decimal LimiteAlimentacaoDia = 60m;
    private const decimal LimiteTransporteDia = 80m;
    private const decimal LimiteAlimentacaoDiaViagem = 90m;
    private const decimal LimiteTransporteDiaViagem = 120m;

    public ContextoAvaliacao Aplicar(ContextoAvaliacao contexto)
    {
        var saldos = new Dictionary<(DateOnly Data, string Categoria), decimal>();

        var itens = contexto.Itens
            .Select(item => Avaliar(item, contexto.Viagem, saldos))
            .ToList();

        return contexto with { Itens = itens };
    }

    private static ItemAvaliado Avaliar(
        ItemAvaliado item,
        bool viagem,
        Dictionary<(DateOnly Data, string Categoria), decimal> saldos)
    {
        if (item.Situacao != Situacao.Ativo)
            return item;

        var limite = LimiteDaCategoria(item.Categoria, viagem);
        if (limite is null)
            return item;

        var chave = (item.Despesa.Data, item.Categoria);
        var saldo = saldos.GetValueOrDefault(chave, limite.Value);
        var pago = Math.Min(item.Despesa.Valor, saldo);
        saldos[chave] = saldo - pago;

        return item with
        {
            Situacao = pago > 0m ? Situacao.Aprovado : Situacao.Recusado,
            ValorReembolsavel = pago
        };
    }

    private static decimal? LimiteDaCategoria(string categoria, bool viagem) =>
        categoria switch
        {
            "alimentacao" => viagem ? LimiteAlimentacaoDiaViagem : LimiteAlimentacaoDia,
            "transporte_urbano" => viagem ? LimiteTransporteDiaViagem : LimiteTransporteDia,
            _ => null
        };
}
