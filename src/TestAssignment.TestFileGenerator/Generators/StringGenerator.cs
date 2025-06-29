using System.Text;
using TestAssignment.TestFileGenerator.Interfaces;

namespace TestAssignment.TestFileGenerator;

public sealed class StringGenerator : IStringGenerator
{
    private const int MinStringLength = 1;
    private const int MaxStringLength = 250;
    private const int PercentOfRepeatingStrings = 20;
    private const int CountOfRepeatingStrings = 1000;
    private const string AllowedChars = " ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    private static readonly byte[] AllowedBytes = Encoding.UTF8.GetBytes(AllowedChars);
    private static readonly int AllowBytesLength = AllowedChars.Length;

    private readonly byte[][] _repeatingStrings;

    public StringGenerator()
    {
        // Generate repeating string on object creation
        _repeatingStrings = new byte[CountOfRepeatingStrings][];
        for (int i = 0; i < CountOfRepeatingStrings; i++)
        {
            _repeatingStrings[i] = GenerateRandomByteArray();
        }
    }

    public byte[] Generate()
    {
        if (Random.Shared.Next(0, 100) < PercentOfRepeatingStrings)
        {
            return _repeatingStrings[Random.Shared.Next(0, CountOfRepeatingStrings)];
        }

        var generatedArray = GenerateRandomByteArray();
        return generatedArray;
    }

    private static byte[] GenerateRandomByteArray()
    {
        int length = Random.Shared.Next(MinStringLength, MaxStringLength + 1);
        byte[] byteArray = new byte[length];

        // Ensure the first and the last bytes are not spaces
        byteArray[0] = AllowedBytes[Random.Shared.Next(1, AllowBytesLength)]; 
        byteArray[length - 1] = AllowedBytes[Random.Shared.Next(1, AllowBytesLength)]; 

        for (int i = 1; i < length - 1; i++)
        {
            byteArray[i] = AllowedBytes[Random.Shared.Next(0, AllowBytesLength)];
        }

        return byteArray;
    }
}