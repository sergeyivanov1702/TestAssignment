using BenchmarkDotNet.Attributes;
using TestAssignment.TestFileGenerator.Generators;

namespace TestAssignment.TestFileGenerator.Benchmark;

[LongRunJob]
public class StringGeneratorBenchmarks
{
    private StringGenerator _stringByteGenerator = default!;

    [GlobalSetup]
    public void Setup()
    {
        _stringByteGenerator = new StringGenerator();
    }

    [Benchmark(Baseline = true)]
    public async Task GenerateStringAsByteArray()
    {
        var threadCount = Environment.ProcessorCount - 1;
        var tasks = new List<Task>();
        for (int i = 0; i < threadCount; i++)
        {
            tasks.Add(Task.Run(() => { var result = _stringByteGenerator.Generate(); }));
        }

        await Task.WhenAll(tasks);
    }

}
