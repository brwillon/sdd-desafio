namespace Reembolso.Core;

public sealed record Despesa(
    string Id,
    DateOnly Data,
    string Categoria,
    string Fornecedor,
    decimal Valor,
    bool TemNotaFiscal,
    string? Descricao);
