using System.Globalization;

namespace Reembolso.Core;

internal static class Moeda
{
    public static string Formatar(decimal valor) =>
        valor.ToString("0.00", CultureInfo.InvariantCulture).Replace('.', ',');
}
