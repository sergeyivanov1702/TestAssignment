using TestAssignment.TestFileGenerator.Interfaces;

namespace TestAssignment.TestFileGenerator;

public sealed class FileGenerator
{
    private readonly ILineGenerator _lineGenerator;

    public FileGenerator(ILineGenerator lineGenerator)
    {
        _lineGenerator = lineGenerator ?? throw new ArgumentNullException(nameof(lineGenerator));
    }
}
