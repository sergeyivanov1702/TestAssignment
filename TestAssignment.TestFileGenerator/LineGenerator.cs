using System.Collections.Concurrent;
using TestAssignment.TestFileGenerator.Interfaces;

namespace TestAssignment.TestFileGenerator;

public sealed class LineGenerator : ILineGenerator
{
    private const int MinNumber = 1;
    private const int MaxNumber = short.MaxValue;

    private readonly IStringGenerator 
    public LineGenerator()
    {
        
    }

    public string GenerateLine()
    {
        return $"{GenerateNumber().ToString()}. {GenerateString()}";
    }

    private int GenerateNumber()
    {
        return Random.Shared.Next(MinNumber, MaxNumber + 1);
    }
}
