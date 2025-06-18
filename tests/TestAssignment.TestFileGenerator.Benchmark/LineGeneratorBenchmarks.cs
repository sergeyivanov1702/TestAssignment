using BenchmarkDotNet.Attributes;
using TestAssignment.TestFileGenerator.Generators;

namespace TestAssignment.TestFileGenerator.Benchmark;

[LongRunJob]
public class LineGeneratorBenchmarks
{
    private readonly LineGenerator _lineByteGenerator;

    public LineGeneratorBenchmarks()
    {
        _lineByteGenerator = new(new StringGenerator());
    }

    [Benchmark(Baseline = true)]
    public async Task GenerateLineAsByteArray()
    {
        var threadCount = Environment.ProcessorCount - 1;
        var tasks = new List<Task>();
        for (int i = 0; i < threadCount; i++)
        {
            tasks.Add(Task.Run(() => { var result = _lineByteGenerator.Generate(); }));
        }

        await Task.WhenAll(tasks);
    }
}
