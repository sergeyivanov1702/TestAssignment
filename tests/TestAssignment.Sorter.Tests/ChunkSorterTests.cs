using FluentAssertions;

namespace TestAssignment.Sorter.Tests;

public sealed class ChunkSorterTests : IDisposable
{
    private readonly string _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    private readonly ChunkSorter _sorter;

    public ChunkSorterTests()
    {
        Directory.CreateDirectory(_tempDirectory);
        _sorter = new ChunkSorter();
    }

    [Fact]
    public void SortAndSave_WhenCalled_ShouldSortChunkAndWriteToFile()
    {
        // Arrange
        var chunk = new List<LineRecord>
        {
            LineRecord.Parse("415. Apple"),
            LineRecord.Parse("3. Apple"),
            LineRecord.Parse("4. Banana"),
            LineRecord.Parse("1. Cherry")
        };

        // Act
        var tempPath = _sorter.SortAndSave(chunk, _tempDirectory);

        // Assert
        Assert.True(File.Exists(tempPath));

        var lines = File.ReadAllLines(tempPath);
        var expected = new[]
        {
            "3. Apple",
            "415. Apple",
            "4. Banana",
            "1. Cherry"
        };

        Assert.Equal(expected, lines);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, true);
        }
    }
}
