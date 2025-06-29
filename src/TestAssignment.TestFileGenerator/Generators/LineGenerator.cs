using System.Text;
using TestAssignment.TestFileGenerator.Interfaces;

namespace TestAssignment.TestFileGenerator;

public sealed class LineGenerator : ILineGenerator
{
    private const string Delimiter = ". ";

    private static readonly byte[] NewLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);
    private static readonly byte[] DelimiterBytes = Encoding.UTF8.GetBytes(Delimiter);

    private readonly IStringGenerator _stringByteGenerator;
    private readonly INumberGenerator _numberByteGenerator;

    public LineGenerator(IStringGenerator stringGenerator, INumberGenerator numberGenerator)
    {
        _stringByteGenerator = stringGenerator ?? throw new ArgumentNullException(nameof(stringGenerator));
        _numberByteGenerator = numberGenerator ?? throw new ArgumentNullException(nameof(numberGenerator));
    }

    public byte[] Generate()
    {
        var numberBytes = _numberByteGenerator.Generate();
        var contentBytes = _stringByteGenerator.Generate();

        var result = new byte[numberBytes.Length + DelimiterBytes.Length + contentBytes.Length + NewLineBytes.Length];

        Buffer.BlockCopy(numberBytes, 0, result, 0, numberBytes.Length);
        Buffer.BlockCopy(DelimiterBytes, 0, result, numberBytes.Length, DelimiterBytes.Length);
        Buffer.BlockCopy(contentBytes, 0, result, numberBytes.Length + DelimiterBytes.Length, contentBytes.Length);
        Buffer.BlockCopy(NewLineBytes, 0, result, numberBytes.Length + DelimiterBytes.Length + contentBytes.Length,
            NewLineBytes.Length); // Finish with a newline byte

        return result;
    }
}
