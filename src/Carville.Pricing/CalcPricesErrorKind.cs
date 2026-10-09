namespace Carville.Pricing;

/// <summary>
/// Причина, по которой не удалось рассчитать цены.
/// </summary>
public enum CalcPricesErrorKind
{
    /// <summary>Цена равна NaN или бесконечности.</summary>
    PriceNotFinite,

    /// <summary>Цена отрицательная.</summary>
    PriceNegative,

    /// <summary>Цена с НДС больше допустимого максимума.</summary>
    PriceTooLarge
}
