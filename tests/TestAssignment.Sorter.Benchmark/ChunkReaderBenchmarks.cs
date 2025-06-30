using BenchmarkDotNet.Attributes;

namespace TestAssignment.Sorter.Benchmark;

public class ChunkReaderBenchmarks
{
    private const string FilePath = "test.txt";
    private ChunkReader _chunkReader = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _chunkReader = new ChunkReader();
    }

    [Benchmark]
    public void ReadChunks()
    {
        long totalRecords = 0L;
        foreach (var chunk in _chunkReader.ReadChunks(FilePath, 1024 * 1024))
        {
            // Simulate processing chunk
            totalRecords += chunk.Count;
        }

        Console.WriteLine($"Total records read: {totalRecords}");
    }
}
