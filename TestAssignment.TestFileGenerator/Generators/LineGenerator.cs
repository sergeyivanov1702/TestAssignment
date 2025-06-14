using TestAssignment.TestFileGenerator.Interfaces;

namespace TestAssignment.TestFileGenerator.Generators;

public sealed class LineGenerator : ILineGenerator
{
    private const int MinNumber = 1;
    private const int MaxNumber = short.MaxValue;
    private readonly IStringGenerator _stringGenerator;
    private static readonly ThreadLocal<Random> _threadLocalRandom = new(() => new Random(Guid.NewGuid().GetHashCode()));

    public LineGenerator(IStringGenerator stringGenerator)
    {
        _stringGenerator = stringGenerator ?? throw new ArgumentNullException(nameof(stringGenerator));
    }

    public string Generate()
    {
        return $"{GenerateNumber().ToString()}. {_stringGenerator.Generate()}";
    }

    private static int GenerateNumber()
    {
        return _threadLocalRandom.Value!.Next(MinNumber, MaxNumber + 1);
    }
}
