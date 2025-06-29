using FluentAssertions;
using Moq;
using System.Text;
using TestAssignment.TestFileGenerator.Interfaces;

namespace TestAssignment.TestFileGenerator.Tests.Generators;

public class LineGeneratorTests
{
    private static readonly byte[] DelimiterBytes = Encoding.UTF8.GetBytes(". ");
    private static readonly byte[] MockStringBytes = Encoding.UTF8.GetBytes("test string");
    private static readonly byte[] MockNumberBytes = Encoding.UTF8.GetBytes("123");
    private readonly Mock<IStringGenerator> _stringGeneratorMock;
    private readonly Mock<INumberGenerator> _numberGeneratorMock;
    private readonly LineGenerator _lineGenerator;

    public LineGeneratorTests()
    {
        _stringGeneratorMock = new Mock<IStringGenerator>();
        _stringGeneratorMock.Setup(s => s.Generate()).Returns(MockStringBytes);
        _numberGeneratorMock = new Mock<INumberGenerator>();
        _numberGeneratorMock.Setup(n => n.Generate()).Returns(MockNumberBytes);
        _lineGenerator = new LineGenerator(_stringGeneratorMock.Object, _numberGeneratorMock.Object);
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
    public void Generate_Whencalled_ResultShouldEndWithNewLine()
    {
        // Act
        var result = _lineGenerator.Generate();

        // Assert
        var newLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);
        result.Should().EndWith(newLineBytes);
    }

    [Fact]
    public void Generate_WhenCalled_ResultShouldContainDelimiter()
    {
        // Act
        var result = _lineGenerator.Generate();

        // Assert
        result.Should().Contain(DelimiterBytes);
    }

    [Fact]
    public void Generate_WhenCalled_ResultShouldStartWithNumber()
    {
        // Act
        var result = _lineGenerator.Generate();

        // Assert
        var delimiterIndex = result.AsSpan().IndexOf(DelimiterBytes);
        delimiterIndex.Should().BeGreaterThan(0);
        var numberPart = result.AsSpan(0, MockNumberBytes.Length).ToArray();
        numberPart.Should().BeEquivalentTo(MockNumberBytes);
    }

    [Fact]
    public void Generate_WhenCalled_ResultShouldContainStringAfterDelimiter()
    {
        // Act
        var result = _lineGenerator.Generate();

        // Assert
        var delimiterIndex = result.AsSpan().IndexOf(DelimiterBytes);
        delimiterIndex.Should().BeGreaterThan(0);
        var numberPart = result.AsSpan(delimiterIndex + DelimiterBytes.Length, MockStringBytes.Length).ToArray();
        numberPart.Should().BeEquivalentTo(MockStringBytes);
    }
}
