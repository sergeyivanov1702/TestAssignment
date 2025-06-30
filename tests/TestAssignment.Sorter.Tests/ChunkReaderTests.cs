using FluentAssertions;

namespace TestAssignment.Sorter.Tests;

public sealed class ChunkReaderTests : IDisposable
{
    private readonly string _tempFile;
    private readonly ChunkReader _reader;

    public ChunkReaderTests()
    {
        _tempFile = Path.GetTempFileName();
        _reader = new ChunkReader();
    }

    [Fact]
    public void ReadChunks_FileIsSmallerThanChunkSize_ShouldReadInSingleChunk()
    {
        // Arrange
        var content = new[] { "1. Apple", "2. Banana" };
        File.WriteAllLines(_tempFile, content);

        // Act
        var chunks = _reader.ReadChunks(_tempFile, 1024 * 1024).ToList(); // 1MB chunk size

        // Assert
        Assert.Single(chunks);
        var chunk = chunks.First();
        chunk.Count.Should().Be(2);
        chunk[0].OriginalLine.Should().Be("1. Apple");
        chunk[1].OriginalLine.Should().Be("2. Banana");
    }

    [Fact]
    public void ReadChunks_FileIsLargerThanChunkSize_ShouldReadInMultipleChunks()
    {
        // Arrange
        var content = new[] { "1. A", "2. B", "3. C", "4. D", "5. E" };
        File.WriteAllLines(_tempFile, content);
        
        // Act
        var chunks = _reader.ReadChunks(_tempFile, 12).ToList(); // Chunk size is small enough to fit 2 lines at most

        // Assert
        chunks.Count.Should().BeGreaterThan(1);
    }

    [Fact]
    public void ReadChunks_FileIsEmpty_ShouldReturnNoChunks()
    {
        // Arrange
        File.WriteAllText(_tempFile, string.Empty);

        // Act
        var chunks = _reader.ReadChunks(_tempFile, 1024).ToList();

        // Assert
        chunks.Should().BeEmpty();
    }

    public void Dispose()
    {
        if (File.Exists(_tempFile))
        {
            File.Delete(_tempFile);
        }
    }
}
