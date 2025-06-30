using BenchmarkDotNet.Attributes;

namespace TestAssignment.Sorter.Benchmark;

public class LineRecordBenchmarks
{
    private readonly List<string> _lines = [];

    [GlobalSetup]
    public void Setup()
    {
        StreamReader? sr = null;
        try
        {
            sr = new StreamReader("test.txt");
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine() ?? throw new ArgumentNullException("Line cannot be null.");
                _lines.Add(line);
            }
        }
        finally
        {
            sr?.Dispose();
        }
    }

    [Benchmark(Baseline = true)]
    public void Parse()
    {
        var list = new List<LineRecord>(_lines.Count);
        foreach (var line in _lines)
        {
            list.Add(LineRecord.Parse(line));
        }

        list.Sort();
    }
}
