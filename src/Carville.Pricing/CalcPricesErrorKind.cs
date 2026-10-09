namespace Carville.Pricing;

/// <summary>
/// Причина, по которой не удалось рассчитать цены.
/// </summary>
public enum CalcPricesErrorKind
{
    /// <summary>Цена — NaN или бесконечность.</summary>
    PriceNotFinite,

    /// <summary>Цена отрицательная.</summary>
    PriceNegative,

    /// <summary>Цена больше допустимого максимума.</summary>
    PriceTooLarge
}
