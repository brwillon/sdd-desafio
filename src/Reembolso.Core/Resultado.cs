namespace Reembolso.Core;

public sealed record Resultado(
    string ColaboradorId,
    Competencia Competencia,
    bool Viagem,
    decimal TotalReembolsavel,
    ResumoStatus Resumo,
    IReadOnlyList<ItemAvaliado> Itens);
