using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Carville.Pricing.Terminal;

internal sealed record PriceRequest(decimal Price, ProcNds ProcNds)
{
    public static bool TryRead([NotNullWhen(true)] out PriceRequest? request)
    {
        request = null;
        if (!TryReadPrice(out var price) || !TryReadProcNds(out var procNds))
            return false;

        request = new PriceRequest(price, procNds);
        return true;
    }

    private static bool TryReadPrice(out decimal price)
    {
        while (true)
        {
            price = 0;
            var text = Terminal.Ask("Рекомендованная цена с НДС");
            if (text is null)
                return false;

            if (decimal.TryParse(text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out price))
                return true;

            Terminal.WriteError("Цена должна быть числом.");
        }
    }

    private static bool TryReadProcNds([NotNullWhen(true)] out ProcNds? procNds)
    {
        while (true)
        {
            procNds = null;
            var text = Terminal.Ask("Процент НДС");
            if (text is null)
                return false;

            if (int.TryParse(text, out var value) && ProcNds.TryCreate(value, out procNds))
                return true;

            Terminal.WriteError("Процент НДС должен быть целым числом от 0 до 99.");
        }
    }
}
