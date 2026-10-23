using System.Collections.Immutable;

namespace Reembolso.Core;

public sealed record ItemAvaliado(
    Despesa Despesa,
    string Categoria,
    Situacao Situacao,
    decimal ValorReembolsavel,
    string Justificativa,
    ImmutableArray<string> Regras,
    ImmutableArray<string> Alertas)
{
    public static ItemAvaliado De(Despesa despesa) =>
        new(
            despesa with { Valor = Arredondamento.Aplicar(despesa.Valor) },
            despesa.Categoria,
            Situacao.Ativo,
            0m,
            string.Empty,
            ImmutableArray<string>.Empty,
            ImmutableArray<string>.Empty);
}
