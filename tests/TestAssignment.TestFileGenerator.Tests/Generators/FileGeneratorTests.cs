using FluentAssertions;
using Moq;
using System.Text;
using TestAssignment.TestFileGenerator.Interfaces;

namespace TestAssignment.TestFileGenerator.Tests.Generators;

public class FileGeneratorTests
{
    private static readonly byte[] GeneratedLine = Encoding.UTF8.GetBytes($"1. line{Environment.NewLine}");
    private readonly Mock<ILineGenerator> _lineGeneratorMock;
    private readonly FileGenerator _fileGenerator;
    private readonly string _outputFilePath;

    public FileGeneratorTests()
    {
        _lineGeneratorMock = new Mock<ILineGenerator>();
        _lineGeneratorMock.Setup(s => s.Generate()).Returns(GeneratedLine);
        _fileGenerator = new FileGenerator(_lineGeneratorMock.Object);
        _outputFilePath = Path.GetTempFileName();// Arrange
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GenerateAsync_WithNullOrWhiteSpacePath_ShouldThrowArgumentException(string? path)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _fileGenerator.GenerateAsync(path!, 1024));
    }

    [Fact]
    public async Task GenerateAsync_WithZeroTargetSize_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _fileGenerator.GenerateAsync(_outputFilePath, 0));
    }

    [Fact]
    public async Task GenerateAsync_ShouldCreateFile()
    {
        // Act
        await _fileGenerator.GenerateAsync(_outputFilePath, 10);

        // Assert
        File.Exists(_outputFilePath).Should().BeTrue();

        // Cleanup
        File.Delete(_outputFilePath);
    }

    [Fact]
    public async Task GenerateAsync_ShouldWriteContentToFile()
    {
        // Act
        await _fileGenerator.GenerateAsync(_outputFilePath, 50);

        // Assert
        var fileContent = await File.ReadAllBytesAsync(_outputFilePath);
        fileContent.Length.Should().BeGreaterThan(0);
        fileContent.Should().Contain(GeneratedLine);

        // Cleanup
        File.Delete(_outputFilePath);
    }

    [Fact]
    public async Task GenerateAsync_ShouldGenerateFileOfAtLeastTargetSize()
    {
        // Arrange
        long targetSize = 1024;

        // Act
        await _fileGenerator.GenerateAsync(_outputFilePath, targetSize);

        // Assert
        var fileInfo = new FileInfo(_outputFilePath);
        fileInfo.Length.Should().BeGreaterThanOrEqualTo(targetSize);

        // Cleanup
        File.Delete(_outputFilePath);
    }

    [Fact]
    public async Task GenerateAsync_ShouldNotEndWithNewLine()
    {
        // Arrange
        var newLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);

        // Act
        await _fileGenerator.GenerateAsync(_outputFilePath, 50);

        // Assert
        var fileContent = await File.ReadAllBytesAsync(_outputFilePath);
        var lastBytes = fileContent.Skip(fileContent.Length - newLineBytes.Length).ToArray();
        lastBytes.Should().NotBeEquivalentTo(newLineBytes);

        // Cleanup
        File.Delete(_outputFilePath);
    }
}
