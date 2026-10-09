using System.Diagnostics;

namespace Carville.Pricing.Terminal;

internal static class Terminal
{
    public static void WriteTitle()
    {
        WriteLine("=== Расчёт цен с НДС и без НДС ===", ConsoleColor.Cyan);
        WriteLine("Пустой ввод завершает работу.", ConsoleColor.DarkGray);
        Console.WriteLine();
    }

    public static string? Ask(string label)
    {
        Console.Write($"{label}: ");
        var text = Console.ReadLine();
        return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
    }

    public static void WriteError(string message) => WriteLine($"  {message}", ConsoleColor.Red);

    public static void WriteError(CalcPricesErrorKind errorKind)
    {
        var message = errorKind switch
        {
            CalcPricesErrorKind.PriceNotFinite => "Цена должна быть конечным числом.",
            CalcPricesErrorKind.PriceNegative => "Цена не может быть отрицательной.",
            CalcPricesErrorKind.PriceTooLarge => "Цена слишком большая.",
            _ => throw new UnreachableException()
        };

        WriteError(message);
        Console.WriteLine();
    }

    public static void WriteLine(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}
