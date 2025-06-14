using System.Diagnostics;
using System.Text;

namespace TestAssignment.TestFileGenerator;

public sealed class FileGenerator2
{
    private readonly string[] _possibleStrings;
    private readonly Random _random;
    
    /// <summary>
    /// Creates a new file generator with default string options.
    /// </summary>
    public FileGenerator2()
    {
        _possibleStrings = new[]
        {
            "Apple",
            "Banana is yellow",
            "Cherry is the best",
            "Something something something"
        };
        _random = new Random();
    }

    /// <summary>
    /// Creates a new file generator with custom string options.
    /// </summary>
    /// <param name="possibleStrings">Array of strings to use in the generated file.</param>
    public FileGenerator2(string[] possibleStrings)
    {
        _possibleStrings = possibleStrings ?? throw new ArgumentNullException(nameof(possibleStrings));
        _random = new Random();
    }

    /// <summary>
    /// Generates a file with the specified target size.
    /// </summary>
    /// <param name="filePath">The path where the file will be created.</param>
    /// <param name="targetSizeInBytes">The approximate target file size in bytes.</param>
    /// <param name="useMultithreading">Whether to use multithreading for file generation.</param>
    /// <param name="progress">Optional progress reporter.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The actual size of the generated file in bytes.</returns>
    public async Task<long> GenerateFileAsync(
        string filePath, 
        long targetSizeInBytes, 
        bool useMultithreading = true,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        
        if (targetSizeInBytes <= 0)
            throw new ArgumentException("Target size must be greater than zero", nameof(targetSizeInBytes));

        // Ensure the directory exists
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        if (useMultithreading && targetSizeInBytes > 10_000_000) // Only use multithreading for files larger than 10MB
            return await GenerateFileMultithreadedAsync(filePath, targetSizeInBytes, progress, cancellationToken);
        else
            return await GenerateFileSingleThreadedAsync(filePath, targetSizeInBytes, progress, cancellationToken);
    }

    private async Task<long> GenerateFileSingleThreadedAsync(
        string filePath, 
        long targetSizeInBytes,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 
            bufferSize: 65536, useAsync: true);
        using var writer = new StreamWriter(fileStream, Encoding.UTF8);

        long currentSize = 0;
        var stringBuilder = new StringBuilder(256); // Pre-allocate reasonable capacity
        
        // Report initial progress
        progress?.Report(0);
        
        while (currentSize < targetSizeInBytes)
        {
            // Check for cancellation
            cancellationToken.ThrowIfCancellationRequested();
            
            stringBuilder.Clear();
            int number = _random.Next(1, 100000); // Random number between 1 and 99999
            string text = _possibleStrings[_random.Next(0, _possibleStrings.Length)];
            stringBuilder.Append(number).Append(". ").Append(text);
            string line = stringBuilder.ToString();
            
            await writer.WriteLineAsync(line);
            
            // Update size tracking
            currentSize += Encoding.UTF8.GetByteCount(line) + Environment.NewLine.Length;
            
            // Report progress
            if (progress != null)
            {
                double progressValue = (double)currentSize / targetSizeInBytes;
                progress.Report(Math.Min(progressValue, 0.99)); // Cap at 99% until complete
            }
        }

        await writer.FlushAsync();
        stopwatch.Stop();
        
        // Report completion
        progress?.Report(1.0);
        
        return currentSize;
    }

    private async Task<long> GenerateFileMultithreadedAsync(
        string filePath, 
        long targetSizeInBytes,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // Determine optimal thread count based on available processors
        int processorCount = Environment.ProcessorCount;
        int threadCount = Math.Max(2, Math.Min(processorCount - 1, 8)); 
            
        // Split the target size among threads
        long sizePerThread = targetSizeInBytes / threadCount;
        
        // Generate content in parallel
        var tasks = new List<Task<List<string>>>(threadCount);
        for (int i = 0; i < threadCount; i++)
        {
            long threadSize = (i == threadCount - 1) 
                ? sizePerThread + (targetSizeInBytes % threadCount) // Add remainder to last thread
                : sizePerThread;
            
            tasks.Add(GenerateContentAsync(threadSize, cancellationToken));
        }

        // Wait for all generation tasks to complete
        var contentLists = await Task.WhenAll(tasks);
        
        // Write all content to file
        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true);
        using var writer = new StreamWriter(fileStream, Encoding.UTF8);
        
        long actualSize = 0;
        long totalLines = contentLists.Sum(list => list.Count);
        long linesWritten = 0;
        
        foreach (var contentList in contentLists)
        {
            foreach (var line in contentList)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await writer.WriteLineAsync(line);
                actualSize += Encoding.UTF8.GetByteCount(line) + Environment.NewLine.Length;
                
                // Update progress for writing phase
                linesWritten++;
                progress?.Report((double)linesWritten / totalLines);
            }
        }
        
        await writer.FlushAsync();
        stopwatch.Stop();
        
        return actualSize;
    }

    private async Task<List<string>> GenerateContentAsync(long targetSizeInBytes, CancellationToken cancellationToken)
    {
        return await Task.Run(() => {
            var result = new List<string>();
            long currentSize = 0;
            var stringBuilder = new StringBuilder();
            var localRandom = new Random(Guid.NewGuid().GetHashCode()); // Thread-safe random

            while (currentSize < targetSizeInBytes)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                stringBuilder.Clear();
                int number = localRandom.Next(1, 100000); // Random number between 1 and 99999
                string text = _possibleStrings[localRandom.Next(0, _possibleStrings.Length)];
                stringBuilder.Append(number).Append(". ").Append(text);
                string line = stringBuilder.ToString();
                
                result.Add(line);
                currentSize += Encoding.UTF8.GetByteCount(line) + Environment.NewLine.Length;
            }
            
            return result;
        }, cancellationToken);
    }

    /// <summary>
    /// Runs a performance benchmark for file generation with different configurations.
    /// </summary>
    /// <param name="filePath">Base file path for benchmark files.</param>
    /// <param name="sizes">Array of file sizes to test (in bytes).</param>
    public async Task RunBenchmarkAsync(string filePath, long[] sizes)
    {
        Console.WriteLine("Running benchmark...");
        Console.WriteLine($"{"Size (MB)",-15}{"Single Thread (ms)",-20}{"Multi Thread (ms)",-20}{"Speedup",-10}");
        
        foreach (var size in sizes)
        {
            var singleThreadFilePath = Path.Combine(
                Path.GetDirectoryName(filePath) ?? string.Empty, 
                Path.GetFileNameWithoutExtension(filePath) + $"_single_{size/1024/1024}MB{Path.GetExtension(filePath)}");
            var multiThreadFilePath = Path.Combine(
                Path.GetDirectoryName(filePath) ?? string.Empty, 
                Path.GetFileNameWithoutExtension(filePath) + $"_multi_{size/1024/1024}MB{Path.GetExtension(filePath)}");

            // Test single-threaded
            var swSingle = Stopwatch.StartNew();
            await GenerateFileAsync(singleThreadFilePath, size, false);
            swSingle.Stop();
            var singleTime = swSingle.ElapsedMilliseconds;
            
            // Test multi-threaded
            var swMulti = Stopwatch.StartNew();
            await GenerateFileAsync(multiThreadFilePath, size, true);
            swMulti.Stop();
            var multiTime = swMulti.ElapsedMilliseconds;
            
            // Calculate speedup
            double speedup = singleTime > 0 ? (double)singleTime / multiTime : 0;
            
            Console.WriteLine($"{size / 1_048_576,-15:F2}{singleTime,-20:F0}{multiTime,-20:F0}{speedup,-10:F2}x");
        }
    }
}
