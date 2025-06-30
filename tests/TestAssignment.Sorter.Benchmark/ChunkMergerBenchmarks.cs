using BenchmarkDotNet.Attributes;

namespace TestAssignment.Sorter.Benchmark;

public class ChunkMergerBenchmarks
{
    private ChunkMerger _chunkMerger = null!;
    private string _tempDirectory = null!;
    private readonly List<string> _sortedTempPaths = new();
    private string _outputFilePath = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _chunkMerger = new ChunkMerger();
        _tempDirectory = Path.Combine(Path.GetTempPath(), "merger-benchmarks");
        Directory.CreateDirectory(_tempDirectory);
        _outputFilePath = Path.Combine(_tempDirectory, "output.txt");

        
        // Create sorted chunks to merge
        var chunkReader = new ChunkReader();
        var chunks = chunkReader.ReadChunks("test.txt", 1024 * 1024);

        var chunkSorter = new ChunkSorter();
        foreach (var chunk in chunks)
        {
            var tempPath = chunkSorter.SortAndSave(chunk, _tempDirectory);
            _sortedTempPaths.Add(tempPath);
        }
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        Directory.Delete(_tempDirectory, true);
    }

    [Benchmark(Baseline = true)]
    public void Merge()
    {
        _chunkMerger.Merge(_sortedTempPaths, _outputFilePath);
    }
}
