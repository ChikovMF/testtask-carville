using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Carville.Pricing;

/// <summary>
/// Расчёт цен с НДС и без НДС, содержащих не более 2 знаков после запятой.
/// </summary>
public static class PriceCalculator
{
    private const double MaxInputPrice = 1_000_000_000_000;

    /// <param name="inputPriceWithNds">Рекомендованная цена с НДС.</param>
    /// <param name="procNds">Процент НДС.</param>
    /// <param name="correctedPriceWithNds">Цена с НДС, ближайшая к рекомендованной.</param>
    /// <param name="correctedPriceWithoutNds">Цена без НДС.</param>
    /// <exception cref="ValidationException">Недопустимая цена.</exception>
    public static void CalcPrices(
        double inputPriceWithNds, 
        ProcNds procNds,
        out double correctedPriceWithNds, 
        out double correctedPriceWithoutNds)
    {
        if (TryCalcPrices(inputPriceWithNds, procNds, out var withNds, out var withoutNds, out var errorKind))
        {
            correctedPriceWithNds = withNds.Value;
            correctedPriceWithoutNds = withoutNds.Value;
            return;
        }

        throw errorKind switch
        {
            CalcPricesErrorKind.PriceNotFinite => new ValidationException("Цена должна быть конечным числом."),
            CalcPricesErrorKind.PriceNegative => new ValidationException("Цена не может быть отрицательной."),
            CalcPricesErrorKind.PriceTooLarge => new ValidationException($"Цена не может превышать {MaxInputPrice:N0}."),
            _ => new UnreachableException()
        };
    }

    /// <param name="inputPriceWithNds">Рекомендованная цена с НДС.</param>
    /// <param name="procNds">Процент НДС.</param>
    /// <param name="correctedPriceWithNds">Цена с НДС, ближайшая к рекомендованной; null при ошибке.</param>
    /// <param name="correctedPriceWithoutNds">Цена без НДС; null при ошибке.</param>
    /// <param name="errorKind">Причина ошибки; null при успехе.</param>
    /// <returns>true, если цены рассчитаны.</returns>
    public static bool TryCalcPrices(
        double inputPriceWithNds, 
        ProcNds procNds,
        [NotNullWhen(true)] out double? correctedPriceWithNds,
        [NotNullWhen(true)] out double? correctedPriceWithoutNds,
        [NotNullWhen(false)] out CalcPricesErrorKind? errorKind)
    {
        errorKind = inputPriceWithNds switch
        {
            _ when !double.IsFinite(inputPriceWithNds) => CalcPricesErrorKind.PriceNotFinite,
            < 0 => CalcPricesErrorKind.PriceNegative,
            > MaxInputPrice => CalcPricesErrorKind.PriceTooLarge,
            _ => null
        };

        if (errorKind is not null)
        {
            correctedPriceWithNds = null;
            correctedPriceWithoutNds = null;
            return false;
        }

        // В копейках: withNds * 100 = withoutNds * (100 + procNds).
        // Целые решения: withNds кратна (100 + procNds) / gcd, withoutNds кратна 100 / gcd.
        var gcd = (int)BigInteger.GreatestCommonDivisor(100 + procNds, 100);
        var stepWithNds = (100 + procNds) / gcd;
        var stepWithoutNds = 100 / gcd;

        // Ближайшее кратное шагу; при равенстве расстояний — большее.
        var inputCents = (decimal)inputPriceWithNds * 100;
        var remainder = inputCents % stepWithNds;
        var steps = (inputCents - remainder) / stepWithNds;
        if (remainder * 2 >= stepWithNds)
            steps++;

        correctedPriceWithNds = (double)(steps * stepWithNds / 100);
        correctedPriceWithoutNds = (double)(steps * stepWithoutNds / 100);
        return true;
    }
}
