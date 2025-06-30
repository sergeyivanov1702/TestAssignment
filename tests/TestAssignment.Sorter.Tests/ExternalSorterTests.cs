using Moq;
using TestAssignment.Sorter.Interfaces;

namespace TestAssignment.Sorter.Tests;

public sealed class ExternalSorterTests
{
    private readonly Mock<IChunkReader> _readerMock;
    private readonly Mock<IChunkSorter> _sorterMock;
    private readonly Mock<IChunkMerger> _mergerMock;
    private readonly ExternalSorter _externalSorter;

    public ExternalSorterTests()
    {
        _readerMock = new Mock<IChunkReader>();
        _sorterMock = new Mock<IChunkSorter>();
        _mergerMock = new Mock<IChunkMerger>();
        _externalSorter = new ExternalSorter(_readerMock.Object, _sorterMock.Object, _mergerMock.Object);
    }

    [Fact]
    public void Execute_WhenCalled_ShouldCoordinateChunkingSortingAndMerging()
    {
        // Arrange
        var options = new SortOptions
        {
            InputPath = "input.txt",
            OutputPath = "output.txt",
            ChunkSizeInBytes = 1024,
            TempDirectory = Path.GetTempPath()
        };

        var chunk1 = new List<LineRecord> { LineRecord.Parse("1. A") };
        var chunk2 = new List<LineRecord> { LineRecord.Parse("2. B") };
        var chunks = new List<List<LineRecord>> { chunk1, chunk2 };

        var tempFile1 = "temp1.tmp";
        var tempFile2 = "temp2.tmp";

        _readerMock.Setup(r => r.ReadChunks(options.InputPath, options.ChunkSizeInBytes)).Returns(chunks);
        _sorterMock.Setup(s => s.SortAndSave(chunk1, options.TempDirectory)).Returns(tempFile1);
        _sorterMock.Setup(s => s.SortAndSave(chunk2, options.TempDirectory)).Returns(tempFile2);

        // Act
        _externalSorter.Execute(options);

        // Assert
        _readerMock.Verify(r => r.ReadChunks(options.InputPath, options.ChunkSizeInBytes), Times.Once);
        _sorterMock.Verify(s => s.SortAndSave(chunk1, options.TempDirectory), Times.Once);
        _sorterMock.Verify(s => s.SortAndSave(chunk2, options.TempDirectory), Times.Once);
        _sorterMock.Verify(s => s.SortAndSave(It.IsAny<List<LineRecord>>(), options.TempDirectory), Times.Exactly(2));
        _mergerMock.Verify(m => m.Merge(It.Is<IEnumerable<string>>(files => files.Contains(tempFile1) && files.Contains(tempFile2)), options.OutputPath), Times.Once);
    }
}
