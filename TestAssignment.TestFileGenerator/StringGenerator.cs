using System.Collections.Concurrent;
using TestAssignment.TestFileGenerator.Interfaces;

namespace TestAssignment.TestFileGenerator;

public sealed class StringGenerator : IStringGenerator
{
    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ";
    private const int MinStringLength = 1;
    private const int MaxStringLength = 250;
    private const int PercentOfRepeatingString = 10;
    private const int MaxRepeatingStrings = 1000;
    private static readonly ConcurrentDictionary<int, string> repeatingStringsStore = new();

    public string GenerateString()
    {
        if (!repeatingStringsStore.IsEmpty || Random.Shared.Next(0, 100) < PercentOfRepeatingString)
        {
            var index = Random.Shared.Next(0, repeatingStringsStore.Count);
            if (repeatingStringsStore.TryGetValue(index, out var existingString))
            {
                return existingString;
            }
        }

        var generatedString = GenerateRandomString();
        // TODO For now I don't care if id adds more items than MaxRepeatingStrings
        if (repeatingStringsStore.Count < MaxRepeatingStrings)
        {
            repeatingStringsStore.AddOrUpdate(repeatingStringsStore.Count, generatedString, (_, oldValue) => oldValue);
        }

        return generatedString;
    }

    private static string GenerateRandomString()
    {
        int length = Random.Shared.Next(MinStringLength, MaxStringLength + 1);
        char[] charArray = new char[length];
        for (int i = 0; i < length; i++)
        {
            charArray[i] = chars[Random.Shared.Next(chars.Length)];
        }
        return new string(charArray);
    }
}
