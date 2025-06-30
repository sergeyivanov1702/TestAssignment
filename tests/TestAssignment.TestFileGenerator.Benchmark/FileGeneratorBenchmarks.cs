using BenchmarkDotNet.Attributes;

namespace TestAssignment.TestFileGenerator.Benchmark;

public class FileGeneratorBenchmarks
{
    private const string OutputFileName = "testfile1.txt";
    private const long FileSizeInBytes = 128L * 1024 * 1024; // 128 MB
    private FileGenerator _fileGenerator = default!;

    [GlobalSetup]
    public void Setup()
    { 
        _fileGenerator = new FileGenerator(new LineGenerator(new StringGenerator(), new NumberGenerator()));
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        if (File.Exists(OutputFileName))
        {
            File.Delete(OutputFileName);
        }
    }

    [Benchmark(Baseline = true)]
    public async Task GenerateFile()
    {
        await _fileGenerator.GenerateAsync(OutputFileName, FileSizeInBytes);
    }
}
