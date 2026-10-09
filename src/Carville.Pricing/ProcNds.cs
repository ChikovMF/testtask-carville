using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Carville.Pricing;

/// <summary>
/// Процент НДС: целое число в диапазоне 0..99.
/// Аналог Delphi-типа <c>TProcNDS = 0..99</c>.
/// </summary>
public sealed record ProcNds
{
    private const int MinValue = 0;
    private const int MaxValue = 99;

    public int Value { get; }

    private ProcNds(int value) => Value = value;

    /// <exception cref="ValidationException">Значение вне допустимого диапазона.</exception>
    public static ProcNds Create(int value)
    {
        if (!TryCreate(value, out var procNds))
            throw new ValidationException(
                $"Процент НДС должен быть в диапазоне {MinValue}..{MaxValue}, получено: {value}.");

        return procNds;
    }

    public static bool TryCreate(int value, [NotNullWhen(true)] out ProcNds? procNds)
    {
        if (value is < MinValue or > MaxValue)
        {
            procNds = null;
            return false;
        }

        procNds = new ProcNds(value);
        return true;
    }

    public static implicit operator int(ProcNds procNds) => procNds.Value;

    public override string ToString() => $"{Value}%";
}
