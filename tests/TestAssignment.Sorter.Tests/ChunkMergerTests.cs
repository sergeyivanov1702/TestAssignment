using FluentAssertions;

namespace TestAssignment.Sorter.Tests;

public sealed class ChunkMergerTests : IDisposable
{
    private readonly ChunkMerger _merger = new();
    private readonly List<string> _tempFiles = [];

    [Fact]
    public void Merge_WhenCalled_MergeFiles()
    {
        // Arrange
        var file1 = CreateTempFile(["1. Apple", "5. Orange"]);
        var file2 = CreateTempFile(["2. Banana", "4. Mango"]);
        var file3 = CreateTempFile(["3. Cherry"]);
        var outputPath = Path.GetTempFileName();
        _tempFiles.Add(outputPath);

        var sortedTempPaths = new List<string> { file1, file2, file3 };

        // Act
        _merger.Merge(sortedTempPaths, outputPath);

        // Assert
        var expected = new[]
        {
            "1. Apple",
            "2. Banana",
            "3. Cherry",
            "4. Mango",
            "5. Orange"
        };
        var actual = File.ReadAllLines(outputPath);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Merge_WhenCalled_ShouldSortByTextFirstThenByNumber()
    {
        // Arrange
        var file1 = CreateTempFile(["415. Apple", "30432. Something something something"]);
        var file2 = CreateTempFile(["1. Apple", "2. Banana is yellow", "32. Cherry is the best"]);
        var outputPath = Path.GetTempFileName();
        _tempFiles.Add(outputPath);

        var sortedTempPaths = new List<string> { file1, file2 };

        // Act
        _merger.Merge(sortedTempPaths, outputPath);

        // Assert
        var expected = new[]
        {
            "1. Apple",
            "415. Apple",
            "2. Banana is yellow",
            "32. Cherry is the best",
            "30432. Something something something"
        };
        var actual = File.ReadAllLines(outputPath);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Merge_WithEmptyFiles_ShouldSkipThem()
    {
        // Arrange
        var file1 = CreateTempFile(["1. Apple", "5. Orange"]);
        var file2 = CreateTempFile([]); // Empty file
        var file3 = CreateTempFile(["3. Cherry"]);
        var outputPath = Path.GetTempFileName();
        _tempFiles.Add(outputPath);

        var sortedTempPaths = new List<string> { file1, file2, file3 };

        // Act
        _merger.Merge(sortedTempPaths, outputPath);

        // Assert
        var expected = new[]
        {
            "1. Apple",
            "3. Cherry",
            "5. Orange"
        };
        var actual = File.ReadAllLines(outputPath);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Merge_WithNoInputFiles_ShouldCreateEmptyOutputFile()
    {
        // Arrange
        var outputPath = Path.GetTempFileName();
        _tempFiles.Add(outputPath);

        // Act
        _merger.Merge([], outputPath);

        // Assert
        var actual = File.ReadAllLines(outputPath);
        actual.Should().BeEmpty();
    }

    private string CreateTempFile(string[] content)
    {
        var path = Path.GetTempFileName();
        File.WriteAllLines(path, content);
        _tempFiles.Add(path);
        return path;
    }

    public void Dispose()
    {
        foreach (var file in _tempFiles)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }
}
