using FluentAssertions;

namespace TestAssignment.TestFileGenerator.Tests.Generators;

public class StringGeneratorTests
{
    private readonly StringGenerator _stringGenerator = new();

    [Fact]
    public void Generate_WhenCalled_ShouldReturnNonEmptyByteArray()
    {
        // Act
        var result = _stringGenerator.Generate();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public void Generate_WhenCalled_ShouldReturnArrayWithinLengthLimits()
    {
        // Act
        var result = _stringGenerator.Generate();

        // Assert
        result.Length.Should().BeInRange(1, 250);
    }

    [Fact]
    public void Generate_WhenCalled_ShouldReturnArrayWithAllowedBytes()
    {
        // Arrange
        string allowedChars = " ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        var allowedBytes = System.Text.Encoding.UTF8.GetBytes(allowedChars);

        // Act
        var result = _stringGenerator.Generate();

        // Assert
        result.Should().OnlyContain(b => allowedBytes.Contains(b));
    }

    [Fact]
    public void Generate_WhenCalled_ShouldNotStartWithSpace()
    {
        // Act
        var result = _stringGenerator.Generate();

        // Assert
        result.Should().NotBeEmpty();
        result[0].Should().NotBe((byte)' ');
    }

    [Fact]
    public void Generate_WhenCalled_ShouldNotFinishWithSpace()
    {
        // Act
        var result = _stringGenerator.Generate();

        // Assert
        result.Should().NotBeEmpty();
        result[result.Length - 1].Should().NotBe((byte)' ');
    }
}
