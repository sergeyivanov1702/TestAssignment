using BenchmarkDotNet.Attributes;

namespace TestAssignment.TestFileGenerator.Benchmark;

public class LineGeneratorBenchmarks
{
    private LineGenerator _lineByteGenerator = default!;

    [GlobalSetup]
    public void Setup()
    {
        _lineByteGenerator = new(new StringGenerator(), new NumberGenerator());
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
