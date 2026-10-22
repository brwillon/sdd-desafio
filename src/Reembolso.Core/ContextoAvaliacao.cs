namespace Reembolso.Core;

public sealed record ContextoAvaliacao(
    Competencia Competencia,
    bool Viagem,
    IReadOnlyList<ItemAvaliado> Itens);
