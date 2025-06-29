using BenchmarkDotNet.Attributes;

namespace TestAssignment.TestFileGenerator.Benchmark;

public class FileGeneratorBenchmarks
{
    private const long FileSizeInBytes = 128L * 1024 * 1024; // 128 MB
    private FileGenerator _fileGenerator = default!;

    [GlobalSetup]
    public void Setup()
    { 
        _fileGenerator = new FileGenerator(new LineGenerator(new StringGenerator(), new NumberGenerator()));
    }

    [Benchmark(Baseline = true)]
    public async Task GenerateFile()
    {
        const string outputFilePath = "testfile1.txt";
        await _fileGenerator.GenerateAsync(outputFilePath, FileSizeInBytes);
    }
}
