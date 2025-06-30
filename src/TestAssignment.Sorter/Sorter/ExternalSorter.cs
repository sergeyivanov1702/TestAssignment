using System.Collections.Concurrent;
using System.Diagnostics;
using TestAssignment.Sorter.Interfaces;

namespace TestAssignment.Sorter;

public sealed class ExternalSorter : IExternalSorter
{
    private readonly IChunkReader _reader;
    private readonly IChunkSorter _sorter;
    private readonly IChunkMerger _merger;

    public ExternalSorter(IChunkReader reader, IChunkSorter sorter, IChunkMerger merger)
    {
        _reader = reader;
        _sorter = sorter;
        _merger = merger;
    }

    public void Execute(SortOptions options)
    {
        var stopwatch = Stopwatch.StartNew();
        var tempFiles = new ConcurrentBag<string>();

        Parallel.ForEach(
            _reader.ReadChunks(options.InputPath, options.ChunkSizeInBytes),
            new ParallelOptions { MaxDegreeOfParallelism = options.MaxDegreeOfParallelism },
            chunk =>
            {
                string tempPath = _sorter.SortAndSave(chunk, options.TempDirectory);
                tempFiles.Add(tempPath);
            });
        Console.WriteLine($"Chunks are sorted. Time taken: {stopwatch.Elapsed.TotalSeconds} seconds");

        _merger.Merge(tempFiles, options.OutputPath);
        Console.WriteLine($"Sorting complete. Time taken: {stopwatch.Elapsed.TotalSeconds} seconds");

        if (options.KeepTempFiles)
        {
            Console.WriteLine($"Temporary files are kept in {options.TempDirectory}");
            return;
        }

        foreach (var path in tempFiles)
        {
            try { File.Delete(path); }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file {path}: {ex.Message}");
            }
        }
    }
}
