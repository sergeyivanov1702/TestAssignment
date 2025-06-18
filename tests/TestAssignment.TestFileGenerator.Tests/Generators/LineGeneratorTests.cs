using FluentAssertions;
using Moq;
using System.Text;
using TestAssignment.TestFileGenerator.Generators;
using TestAssignment.TestFileGenerator.Interfaces;

namespace TestAssignment.TestFileGenerator.Tests.Generators;

public class LineGeneratorTests
{
    private static readonly byte[] DelimiterBytes = Encoding.UTF8.GetBytes(". ");
    private readonly Mock<IStringGenerator> _stringGeneratorMock;
    private readonly LineGenerator _lineGenerator;

    public LineGeneratorTests()
    {
        _stringGeneratorMock = new Mock<IStringGenerator>();
        _stringGeneratorMock.Setup(s => s.Generate()).Returns(Encoding.UTF8.GetBytes("test string"));
        _lineGenerator = new LineGenerator(_stringGeneratorMock.Object);
    }

    [Fact]
    public void Generate_WhenCalled_ShouldReturnNonEmptyByteArray()
    {
        // Act
        var result = _lineGenerator.Generate();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public void Generate_Whencalled_ShouldEndWithNewLine()
    {
        // Act
        var result = _lineGenerator.Generate();

        // Assert
        var newLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);
        result.Should().EndWith(newLineBytes);
    }

    [Fact]
    public void Generate_WhenCalled_ShouldContainDelimiter()
    {
        // Act
        var result = _lineGenerator.Generate();

        // Assert
        result.Should().Contain(DelimiterBytes);
    }

    [Fact]
    public void Generate_WhenCalled_ShouldStartWithNumberWithinLengthLimits()
    {
        // Act
        var result = _lineGenerator.Generate();

        // Assert
        var delimiterIndex = new ReadOnlySpan<byte>(result).IndexOf(DelimiterBytes);
        delimiterIndex.Should().BeInRange(1, 5);
    }

    [Fact]
    public void Generate_WhenCalled_ShouldNotStartWithZero()
    {
        // Act
        var result = _lineGenerator.Generate();

        // Assert
        result.Should().NotBeEmpty();
        result[0].Should().NotBe((byte)'0');
    }

    [Fact]
    public void Generate_WhenCalled_ShouldStartWithValidNumber()
    {
        // Act
        var result = _lineGenerator.Generate();

        // Assert
        var delimiterIndex = result.AsSpan().IndexOf(DelimiterBytes);
        delimiterIndex.Should().BeGreaterThan(0);
        var numberPart = result.AsSpan(0, delimiterIndex).ToArray();
        numberPart.Should().OnlyContain(b => b >= (byte)'0' && b <= (byte)'9');
    }
}
