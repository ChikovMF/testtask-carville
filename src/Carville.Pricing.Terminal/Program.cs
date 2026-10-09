using System.Globalization;
using System.Text;
using Carville.Pricing.Terminal;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

Terminal.WriteTitle();

while (PriceRequest.TryRead(out var request))
{
    if (PriceReport.TryCreate(request, out var report, out var errorKind))
        report.Print();
    else
        Terminal.WriteError(errorKind.Value);
}
