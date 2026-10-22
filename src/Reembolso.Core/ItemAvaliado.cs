using System.Collections.Immutable;

namespace Reembolso.Core;

public sealed record ItemAvaliado(
    Despesa Despesa,
    Situacao Situacao,
    decimal ValorReembolsavel,
    string Justificativa,
    ImmutableArray<string> Regras)
{
    public static ItemAvaliado De(Despesa despesa) =>
        new(despesa, Situacao.Ativo, 0m, string.Empty, ImmutableArray<string>.Empty);
}
