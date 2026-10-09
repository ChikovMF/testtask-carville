using System.Diagnostics.CodeAnalysis;

namespace Carville.Pricing.Terminal;

internal sealed record PriceReport(PriceRequest Request, double WithNds, double WithoutNds)
{
    /// <summary>Множитель НДС: 1 + процент / 100.</summary>
    public decimal Multiplier => 1 + Request.ProcNds.Value / 100m;

    /// <summary>Цена без НДС, умноженная на множитель.</summary>
    public decimal Check => (decimal)WithoutNds * Multiplier;

    /// <summary>Проверка совпала с ценой с НДС.</summary>
    public bool IsCorrect => Check == (decimal)WithNds;

    /// <summary>Разница между рассчитанной и введённой ценой с НДС.</summary>
    public decimal Deviation => (decimal)WithNds - Request.Price;

    public static bool TryCreate(
        PriceRequest request,
        [NotNullWhen(true)] out PriceReport? report,
        [NotNullWhen(false)] out CalcPricesErrorKind? errorKind)
    {
        report = null;
        if (!PriceCalculator.TryCalcPrices((double)request.Price, request.ProcNds,
                out var withNds, out var withoutNds, out errorKind))
            return false;

        report = new PriceReport(request, withNds.Value, withoutNds.Value);
        return true;
    }

    public void Print()
    {
        Console.WriteLine($"  Цена с НДС:   {WithNds:0.00}");
        Console.WriteLine($"  Цена без НДС: {WithoutNds:0.00}");
        Console.WriteLine($"  Отклонение:   {Deviation}");
        Console.Write($"  Проверка:     {WithoutNds:0.00} × {Multiplier:0.00} = {Check:0.00##} ");
        Terminal.WriteLine(IsCorrect ? "✓" : "✗", IsCorrect ? ConsoleColor.Green : ConsoleColor.Red);
        Console.WriteLine();
    }
}
