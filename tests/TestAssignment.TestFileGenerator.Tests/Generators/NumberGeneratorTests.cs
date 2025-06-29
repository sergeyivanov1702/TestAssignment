using FluentAssertions;

namespace TestAssignment.TestFileGenerator.Tests.Generators;

public class NumberGeneratorTests
{
  private readonly NumberGenerator _numberGenerator = new();

  [Fact]
  public void Generate_WhenCalled_ShouldReturnNonEmptyByteArray()
  {
    // Act
    var result = _numberGenerator.Generate();

    // Assert
    result.Should().NotBeNull();
    result.Should().NotBeEmpty();
  }

  [Fact]
  public void Generate_WhenCalled_ShouldReturnNumberWithinLengthLimits()
  {
    // Act
    var result = _numberGenerator.Generate();

    // Assert
    result.Length.Should().BeInRange(1, 5);
  }

  [Fact]
  public void Generate_WhenCalled_ShouldNotStartWithZero()
  {
    // Act
    var result = _numberGenerator.Generate();

    // Assert
    result.Should().NotBeEmpty();
    if (result.Length > 1)
    {
      result[0].Should().NotBe((byte)'0');
    }
  }

  [Fact]
  public void Generate_WhenCalled_ShouldContainOnlyDigits()
  {
    // Act
    var result = _numberGenerator.Generate();

    // Assert
    result.Should().OnlyContain(b => b >= (byte)'0' && b <= (byte)'9');
  }
}
