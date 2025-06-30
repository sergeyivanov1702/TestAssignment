using BenchmarkDotNet.Attributes;

namespace TestAssignment.TestFileGenerator.Benchmark;

public class NumberGeneratorBenchmarks
{
    private NumberGenerator _numberGenerator = default!;

    [GlobalSetup]
    public void Setup()
    {
        _numberGenerator = new NumberGenerator();
    }

    [Benchmark(Baseline = true)]
    public async Task GenerateNumber()
    {
        var threadCount = Environment.ProcessorCount - 1;
        var tasks = new List<Task>();
        for (int i = 0; i < threadCount; i++)
        {
            tasks.Add(Task.Run(() => { var result = _numberGenerator.Generate(); }));
        }

        await Task.WhenAll(tasks);
    }
}
