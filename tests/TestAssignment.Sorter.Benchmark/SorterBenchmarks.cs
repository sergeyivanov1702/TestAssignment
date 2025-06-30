using BenchmarkDotNet.Attributes;

namespace TestAssignment.Sorter.Benchmark;

[ShortRunJob]
public class SorterBenchmarks
{
    private SortOptions _sortOptions = default!;
    private ExternalSorter _sorter = default!;

    [GlobalSetup]
    public void Setup()
    {
        var tmpDir = Path.Combine(Path.GetTempPath(), "sorter-benchmarks");
        _sortOptions = new SortOptions
        {
            InputPath = "test.txt",
            OutputPath = Path.Combine(tmpDir, "sorted.txt"),
            ChunkSizeInBytes = 1024 * 1024, // 1 MB per chunk
            TempDirectory = tmpDir
        };

        Directory.CreateDirectory(_sortOptions.TempDirectory);

        var reader = new ChunkReader();
        var sorter = new ChunkSorter();
        var merger = new ChunkMerger();
        _sorter = new ExternalSorter(reader, sorter, merger);
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        Directory.Delete(_sortOptions.TempDirectory, true);
    }

    [Benchmark(Baseline = true)]
    public void Execute()
    {
        _sorter.Execute(_sortOptions);
    }
}
