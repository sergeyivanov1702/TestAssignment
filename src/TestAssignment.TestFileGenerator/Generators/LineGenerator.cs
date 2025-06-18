using System.Text;
using TestAssignment.TestFileGenerator.Interfaces;

namespace TestAssignment.TestFileGenerator.Generators;

public sealed class LineGenerator : ILineGenerator
{
    private const int MinNumberLength = 1;
    private const int MaxNumberLength = 5;

    private static readonly byte[] NewLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);
    private static readonly byte[] DelimiterBytes = Encoding.UTF8.GetBytes(". ");
    private static readonly byte[] NumberBytes = Encoding.UTF8.GetBytes("0123456789");

    private readonly IStringGenerator _stringByteGenerator;

    public LineGenerator(IStringGenerator stringGenerator)
    {
        _stringByteGenerator = stringGenerator ?? throw new ArgumentNullException(nameof(stringGenerator));
    }

    public byte[] Generate()
    {
        var numberBytes = GenerateNumberBytes();
        var contentBytes = _stringByteGenerator.Generate();

        var result = new byte[numberBytes.Length + DelimiterBytes.Length + contentBytes.Length + NewLineBytes.Length];

        Buffer.BlockCopy(numberBytes, 0, result, 0, numberBytes.Length);
        Buffer.BlockCopy(DelimiterBytes, 0, result, numberBytes.Length, DelimiterBytes.Length);
        Buffer.BlockCopy(contentBytes, 0, result, numberBytes.Length + DelimiterBytes.Length, contentBytes.Length);
        Buffer.BlockCopy(NewLineBytes, 0, result, numberBytes.Length + DelimiterBytes.Length + contentBytes.Length,
            NewLineBytes.Length); // Finish with a newline byte

        return result;
    }

    private static byte[] GenerateNumberBytes()
    {
        int length = Random.Shared.Next(MinNumberLength, MaxNumberLength + 1);
        byte[] result = new byte[length];

        result[0] = NumberBytes[Random.Shared.Next(1, NumberBytes.Length)]; // Ensure first digit is not zero
        for (int i = 1; i < length; i++)
        {
            result[i] = NumberBytes[Random.Shared.Next(0, NumberBytes.Length)];
        }

        return result;
    }
}
