using BenchmarkDotNet.Attributes;
using TestAssignment.TestFileGenerator.Generators;

namespace TestAssignment.TestFileGenerator.Benchmark;

[MemoryDiagnoser]
public class LineGeneratorBenchmarks
{
    private readonly LineGenerator _lineGenerator;
    public LineGeneratorBenchmarks()
    {
        var stringGenerator = new StringGenerator();
        _lineGenerator = new LineGenerator(stringGenerator);
    }

    [Benchmark(Baseline = true)]
    public string GenerateLine()
    {
        return _lineGenerator.Generate();
    }
}
