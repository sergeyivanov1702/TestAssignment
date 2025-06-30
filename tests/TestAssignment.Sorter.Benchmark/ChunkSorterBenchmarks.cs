using BenchmarkDotNet.Attributes;

namespace TestAssignment.Sorter.Benchmark;

public class ChunkSorterBenchmarks
{
    private readonly List<LineRecord> _chunk = [];
    private ChunkSorter _chunkSorter = null!;
    private string _tempDirectory = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _chunkSorter = new ChunkSorter();
        _tempDirectory = Path.Combine(Path.GetTempPath(), "sorter-benchmarks");
        Directory.CreateDirectory(_tempDirectory);

        using var reader = new StreamReader("test.txt");
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line != null)
            {
                _chunk.Add(LineRecord.Parse(line));
            }
        }
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        Directory.Delete(_tempDirectory, true);
    }

    [Benchmark]
    public void SortAndSave()
    {
        _chunkSorter.SortAndSave(_chunk, _tempDirectory);
    }
}
