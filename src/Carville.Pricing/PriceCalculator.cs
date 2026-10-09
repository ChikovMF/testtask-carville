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
    // double хранит без искажений не более 15 значащих цифр.
    private const long MaxPriceCents = 999_999_999_999_999;

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
            CalcPricesErrorKind.PriceTooLarge => new ValidationException($"Цена с НДС не может превышать {MaxPriceCents / 100m:N2}."),
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
        correctedPriceWithNds = null;
        correctedPriceWithoutNds = null;

        if (!double.IsFinite(inputPriceWithNds))
        {
            errorKind = CalcPricesErrorKind.PriceNotFinite;
            return false;
        }

        if (inputPriceWithNds < 0)
        {
            errorKind = CalcPricesErrorKind.PriceNegative;
            return false;
        }

        // В копейках: withNds * 100 = withoutNds * (100 + procNds).
        // Целые решения: withNds кратна stepWithNds, withoutNds кратна stepWithoutNds.
        var gcd = (int)BigInteger.GreatestCommonDivisor(100 + procNds, 100);
        var stepWithNds = (100 + procNds) / gcd;
        var stepWithoutNds = 100 / gcd;

        var steps = RoundToSteps(inputPriceWithNds, stepWithNds);
        var priceWithNdsCents = steps * stepWithNds;

        if (priceWithNdsCents > MaxPriceCents)
        {
            errorKind = CalcPricesErrorKind.PriceTooLarge;
            return false;
        }

        correctedPriceWithNds = (double)priceWithNdsCents / 100;
        correctedPriceWithoutNds = (double)(steps * stepWithoutNds) / 100;
        errorKind = null;
        return true;
    }

    // Число шагов stepCents, ближайшее к price; при равенстве расстояний берётся большее.
    // Считается точно: price представляется как mantissa * 2^exponent.
    private static BigInteger RoundToSteps(double price, int stepCents)
    {
        if (price == 0)
            return 0;

        var exponent = Math.ILogB(price) - 52;
        var mantissa = new BigInteger(Math.ScaleB(price, -exponent));

        // steps = price * 100 / stepCents = numerator / denominator
        var numerator = mantissa * 100;
        var denominator = new BigInteger(stepCents);
        if (exponent > 0)
            numerator <<= exponent;
        else
            denominator <<= -exponent;

        var steps = BigInteger.DivRem(numerator, denominator, out var remainder);
        return remainder * 2 >= denominator ? steps + 1 : steps;
    }
}
