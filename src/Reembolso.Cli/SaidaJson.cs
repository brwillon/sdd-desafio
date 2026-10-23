using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Reembolso.Core;

namespace Reembolso.Cli;

public static class SaidaJson
{
    private static readonly JsonSerializerOptions Opcoes = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string Serializar(Resultado resultado) =>
        JsonSerializer.Serialize(ResultadoDto.De(resultado), Opcoes);

    private sealed class DuasCasas : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type tipo, JsonSerializerOptions opcoes) =>
            reader.GetDecimal();

        public override void Write(Utf8JsonWriter writer, decimal valor, JsonSerializerOptions opcoes) =>
            writer.WriteRawValue(valor.ToString("0.00", CultureInfo.InvariantCulture));
    }

    private sealed record ResultadoDto(
        [property: JsonPropertyName("colaborador_id"), JsonPropertyOrder(1)] string ColaboradorId,
        [property: JsonPropertyName("competencia"), JsonPropertyOrder(2)] string Competencia,
        [property: JsonPropertyName("viagem"), JsonPropertyOrder(3)] bool Viagem,
        [property: JsonPropertyName("total_reembolsavel"), JsonPropertyOrder(4), JsonConverter(typeof(DuasCasas))] decimal TotalReembolsavel,
        [property: JsonPropertyName("resumo"), JsonPropertyOrder(5)] ResumoDto Resumo,
        [property: JsonPropertyName("itens"), JsonPropertyOrder(6)] IReadOnlyList<ItemDto> Itens)
    {
        public static ResultadoDto De(Resultado r) => new(
            r.ColaboradorId,
            $"{r.Competencia.Ano:0000}-{r.Competencia.Mes:00}",
            r.Viagem,
            r.TotalReembolsavel,
            new ResumoDto(r.Resumo.Aprovado, r.Resumo.Recusado, r.Resumo.EmRevisao),
            r.Itens.Select(ItemDto.De).ToList());
    }

    private sealed record ResumoDto(
        [property: JsonPropertyName("aprovado"), JsonPropertyOrder(1)] int Aprovado,
        [property: JsonPropertyName("recusado"), JsonPropertyOrder(2)] int Recusado,
        [property: JsonPropertyName("em_revisao"), JsonPropertyOrder(3)] int EmRevisao);

    private sealed record ItemDto(
        [property: JsonPropertyName("id"), JsonPropertyOrder(1)] string Id,
        [property: JsonPropertyName("categoria"), JsonPropertyOrder(2)] string Categoria,
        [property: JsonPropertyName("status"), JsonPropertyOrder(3)] string Status,
        [property: JsonPropertyName("valor_solicitado"), JsonPropertyOrder(4), JsonConverter(typeof(DuasCasas))] decimal ValorSolicitado,
        [property: JsonPropertyName("valor_reembolsavel"), JsonPropertyOrder(5), JsonConverter(typeof(DuasCasas))] decimal ValorReembolsavel,
        [property: JsonPropertyName("justificativa"), JsonPropertyOrder(6)] string Justificativa,
        [property: JsonPropertyName("regras"), JsonPropertyOrder(7)] IReadOnlyList<string> Regras,
        [property: JsonPropertyName("alertas"), JsonPropertyOrder(8)] IReadOnlyList<string> Alertas)
    {
        public static ItemDto De(ItemAvaliado i) => new(
            i.Despesa.Id,
            i.Categoria,
            NomeDoStatus(i.Situacao),
            i.Despesa.Valor,
            i.ValorReembolsavel,
            i.Justificativa,
            i.Regras.ToList(),
            i.Alertas.ToList());

        private static string NomeDoStatus(Situacao s) => s switch
        {
            Situacao.Aprovado => "aprovado",
            Situacao.Recusado => "recusado",
            Situacao.EmRevisao => "em_revisao",
            _ => throw new InvalidOperationException($"Situação {s} não aparece na saída.")
        };
    }
}
