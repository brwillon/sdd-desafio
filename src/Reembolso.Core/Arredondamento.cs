namespace Reembolso.Core;

public static class Arredondamento
{
    public static decimal Aplicar(decimal valor) =>
        Math.Round(valor, 2, MidpointRounding.ToEven);
}
