using BenchmarkDotNet.Attributes;
using TestAssignment.TestFileGenerator.Generators;

namespace TestAssignment.TestFileGenerator.Benchmark;

[MediumRunJob]
public class FileGeneratorBenchmarks
{
    private const long FileSizeInBytes = 128L * 1024 * 1024; // 128 MB
    private readonly FileGenerator _fileGenerator;

    public FileGeneratorBenchmarks()
    {
        _fileGenerator = new FileGenerator(new LineGenerator(new StringGenerator()));
    }

    [Benchmark(Baseline = true)]
    public async Task GenerateFile()
    {
        const string outputFilePath = "testfile1.txt";
        await _fileGenerator.GenerateAsync(outputFilePath, FileSizeInBytes);
    }
}
