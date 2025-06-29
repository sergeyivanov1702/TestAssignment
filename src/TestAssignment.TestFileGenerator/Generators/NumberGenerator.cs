using System.Text;
using TestAssignment.TestFileGenerator.Interfaces;

namespace TestAssignment.TestFileGenerator;

public sealed class NumberGenerator : INumberGenerator
{
    private const int MinNumberLength = 1;
    private const int MaxNumberLength = 5;

    private static readonly byte[] NumberBytes = Encoding.UTF8.GetBytes("0123456789");

    public byte[] Generate()
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
