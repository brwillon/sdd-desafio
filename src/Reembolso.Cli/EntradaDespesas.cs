using Reembolso.Core;

namespace Reembolso.Cli;

public sealed record EntradaDespesas(
    string ColaboradorId,
    string? ColaboradorNome,
    string? CentroCusto,
    Competencia Competencia,
    IReadOnlyList<Despesa> Despesas);
