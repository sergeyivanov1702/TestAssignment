using BenchmarkDotNet.Attributes;

namespace TestAssignment.TestFileGenerator.Benchmark;

[MemoryDiagnoser, ShortRunJob]
public class LineGeneratorBenchmarks
{
    private readonly LineGenerator _lineGenerator;
    public LineGeneratorBenchmarks()
    {
        _lineGenerator = new LineGenerator();
    }

    [Benchmark(Baseline = true)]
    public string GenerateLine()
    {
        return _lineGenerator.GenerateString();
    }

    [Benchmark]
    public string GenerateLine2()
    {
        return _lineGenerator.GenerateString2();
    }
}
