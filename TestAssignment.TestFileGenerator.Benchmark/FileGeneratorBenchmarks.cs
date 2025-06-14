using BenchmarkDotNet.Attributes;
using TestAssignment.TestFileGenerator.Generators;

namespace TestAssignment.TestFileGenerator.Benchmark;

[MemoryDiagnoser, ShortRunJob]
public class FileGeneratorBenchmarks
{
    private const long FileSizeInBytes = 10 * 1024 * 1024; // 10 MB
    private readonly FileGenerator _fileGenerator;
    private readonly FileGeneratorNew _fileGeneratorNew;

    public FileGeneratorBenchmarks()
    {
        _fileGenerator = new FileGenerator(new LineGenerator(new StringGenerator()));
        _fileGeneratorNew = new FileGeneratorNew(new LineGenerator(new StringGenerator()));
    }

    [Benchmark(Baseline = true)]
    public async Task GenerateFileAsync()
    {
        const string outputFilePath = "testfile.txt";
        await _fileGenerator.GenerateAsync(outputFilePath, FileSizeInBytes);
    }

    public async Task GenerateFileAsyncNew()
    {
        const string outputFilePath = "testfileNew.txt";
        await _fileGeneratorNew.GenerateAsync(outputFilePath, FileSizeInBytes);
    }
}
