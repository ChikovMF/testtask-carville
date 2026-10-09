using System.ComponentModel.DataAnnotations;

namespace Carville.Pricing.Tests;

public class PriceCalculatorTests
{
    [Theory]
    [InlineData(100, 0, 100)]
    [InlineData(100, 7, 99.51)]
    [InlineData(100, 18, 99.71)]
    [InlineData(100, 20, 100.02)]
    [InlineData(100, 99, 99.5)]
    public void CalcPrices_ValidPrice_ReturnsNearestPriceWithNds(double input, int procNds, double expected)
    {
        // Arrange
        var vat = ProcNds.Create(procNds);

        // Act
        PriceCalculator.CalcPrices(input, vat, out var withNds, out _);

        // Assert
        Assert.Equal(expected, withNds);
    }

    [Theory]
    [InlineData(100, 0, 100)]
    [InlineData(100, 7, 93)]
    [InlineData(100, 18, 84.5)]
    [InlineData(100, 20, 83.35)]
    [InlineData(100, 99, 50)]
    public void CalcPrices_ValidPrice_ReturnsPriceWithoutNds(double input, int procNds, double expected)
    {
        // Arrange
        var vat = ProcNds.Create(procNds);

        // Act
        PriceCalculator.CalcPrices(input, vat, out _, out var withoutNds);

        // Assert
        Assert.Equal(expected, withoutNds);
    }

    [Fact]
    public void CalcPrices_PriceInMiddleBetweenSteps_ReturnsLargerPrice()
    {
        // Arrange
        var vat = ProcNds.Create(20);

        // Act
        PriceCalculator.CalcPrices(0.75, vat, out var withNds, out _);

        // Assert
        Assert.Equal(0.78, withNds);
    }

    [Fact]
    public void CalcPrices_PriceJustBelowMiddle_ReturnsSmallerPrice()
    {
        // Arrange
        var vat = ProcNds.Create(20);

        // Act
        PriceCalculator.CalcPrices(0.7499999999999999, vat, out var withNds, out _);

        // Assert
        Assert.Equal(0.72, withNds);
    }

    [Fact]
    public void CalcPrices_PriceCloserToZeroThanToFirstStep_ReturnsZero()
    {
        // Arrange
        var vat = ProcNds.Create(7);

        // Act
        PriceCalculator.CalcPrices(0.40, vat, out var withNds, out _);

        // Assert
        Assert.Equal(0, withNds);
    }

    [Fact]
    public void CalcPrices_NegativeZero_ReturnsPositiveZero()
    {
        // Arrange
        var vat = ProcNds.Create(20);

        // Act
        PriceCalculator.CalcPrices(-0.0, vat, out var withNds, out _);

        // Assert
        Assert.False(double.IsNegative(withNds));
    }

    [Fact]
    public void CalcPrices_LargePriceWithFractionalPart_ReturnsNearestPriceWithNds()
    {
        // Arrange
        var vat = ProcNds.Create(0);

        // Act
        PriceCalculator.CalcPrices(123456789012.3449, vat, out var withNds, out _);

        // Assert
        Assert.Equal(123456789012.34, withNds);
    }

    [Fact]
    public void CalcPrices_MaxPrice_ReturnsMaxPrice()
    {
        // Arrange
        var vat = ProcNds.Create(0);

        // Act
        PriceCalculator.CalcPrices(9999999999999.99, vat, out var withNds, out _);

        // Assert
        Assert.Equal(9999999999999.99, withNds);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(-0.01)]
    [InlineData(1e13)]
    public void CalcPrices_InvalidPrice_ThrowsValidationException(double input)
    {
        // Arrange
        var vat = ProcNds.Create(20);

        // Act
        var act = () => PriceCalculator.CalcPrices(input, vat, out _, out _);

        // Assert
        Assert.Throws<ValidationException>(act);
    }

    [Fact]
    public void TryCalcPrices_ValidPrice_ReturnsTrue()
    {
        // Arrange
        var vat = ProcNds.Create(20);

        // Act
        var result = PriceCalculator.TryCalcPrices(100, vat, out _, out _, out _);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TryCalcPrices_ValidPrice_OutputsPriceWithNds()
    {
        // Arrange
        var vat = ProcNds.Create(20);

        // Act
        PriceCalculator.TryCalcPrices(100, vat, out var withNds, out _, out _);

        // Assert
        Assert.Equal(100.02, withNds);
    }

    [Fact]
    public void TryCalcPrices_ValidPrice_OutputsPriceWithoutNds()
    {
        // Arrange
        var vat = ProcNds.Create(20);

        // Act
        PriceCalculator.TryCalcPrices(100, vat, out _, out var withoutNds, out _);

        // Assert
        Assert.Equal(83.35, withoutNds);
    }

    [Fact]
    public void TryCalcPrices_ValidPrice_OutputsNullErrorKind()
    {
        // Arrange
        var vat = ProcNds.Create(20);

        // Act
        PriceCalculator.TryCalcPrices(100, vat, out _, out _, out var errorKind);

        // Assert
        Assert.Null(errorKind);
    }

    [Fact]
    public void TryCalcPrices_InvalidPrice_ReturnsFalse()
    {
        // Arrange
        var vat = ProcNds.Create(20);

        // Act
        var result = PriceCalculator.TryCalcPrices(-1, vat, out _, out _, out _);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryCalcPrices_InvalidPrice_OutputsNullPriceWithNds()
    {
        // Arrange
        var vat = ProcNds.Create(20);

        // Act
        PriceCalculator.TryCalcPrices(-1, vat, out var withNds, out _, out _);

        // Assert
        Assert.Null(withNds);
    }

    [Fact]
    public void TryCalcPrices_InvalidPrice_OutputsNullPriceWithoutNds()
    {
        // Arrange
        var vat = ProcNds.Create(20);

        // Act
        PriceCalculator.TryCalcPrices(-1, vat, out _, out var withoutNds, out _);

        // Assert
        Assert.Null(withoutNds);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void TryCalcPrices_NotFinitePrice_OutputsPriceNotFinite(double input)
    {
        // Arrange
        var vat = ProcNds.Create(20);

        // Act
        PriceCalculator.TryCalcPrices(input, vat, out _, out _, out var errorKind);

        // Assert
        Assert.Equal(CalcPricesErrorKind.PriceNotFinite, errorKind);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1e300)]
    public void TryCalcPrices_NegativePrice_OutputsPriceNegative(double input)
    {
        // Arrange
        var vat = ProcNds.Create(20);

        // Act
        PriceCalculator.TryCalcPrices(input, vat, out _, out _, out var errorKind);

        // Assert
        Assert.Equal(CalcPricesErrorKind.PriceNegative, errorKind);
    }

    [Theory]
    [InlineData(1e13, 0)]
    [InlineData(double.MaxValue, 0)]
    [InlineData(9999999999999.99, 99)]
    public void TryCalcPrices_PriceAboveMaximum_OutputsPriceTooLarge(double input, int procNds)
    {
        // Arrange
        var vat = ProcNds.Create(procNds);

        // Act
        PriceCalculator.TryCalcPrices(input, vat, out _, out _, out var errorKind);

        // Assert
        Assert.Equal(CalcPricesErrorKind.PriceTooLarge, errorKind);
    }
}
