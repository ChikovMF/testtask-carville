using System.ComponentModel.DataAnnotations;

namespace Carville.Pricing.Tests;

public class ProcNdsTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(20)]
    [InlineData(99)]
    public void Create_ValueInRange_ReturnsProcNdsWithValue(int value)
    {
        // Arrange

        // Act
        var procNds = ProcNds.Create(value);

        // Assert
        Assert.Equal(value, procNds.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(100)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void Create_ValueOutOfRange_ThrowsValidationException(int value)
    {
        // Arrange

        // Act
        var act = () => ProcNds.Create(value);

        // Assert
        Assert.Throws<ValidationException>(act);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(99)]
    public void TryCreate_ValueInRange_ReturnsTrue(int value)
    {
        // Arrange

        // Act
        var result = ProcNds.TryCreate(value, out _);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(99)]
    public void TryCreate_ValueInRange_OutputsProcNdsWithValue(int value)
    {
        // Arrange

        // Act
        ProcNds.TryCreate(value, out var procNds);

        // Assert
        Assert.Equal(value, procNds?.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(100)]
    public void TryCreate_ValueOutOfRange_ReturnsFalse(int value)
    {
        // Arrange

        // Act
        var result = ProcNds.TryCreate(value, out _);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(100)]
    public void TryCreate_ValueOutOfRange_OutputsNull(int value)
    {
        // Arrange

        // Act
        ProcNds.TryCreate(value, out var procNds);

        // Assert
        Assert.Null(procNds);
    }

    [Fact]
    public void Create_SameValue_ReturnsEqualInstances()
    {
        // Arrange
        var expected = ProcNds.Create(20);

        // Act
        var actual = ProcNds.Create(20);

        // Assert
        Assert.Equal(expected, actual);
    }
}
