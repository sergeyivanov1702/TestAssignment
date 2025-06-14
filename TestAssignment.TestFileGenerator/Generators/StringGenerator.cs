using System.Collections.Concurrent;
using TestAssignment.TestFileGenerator.Interfaces;

namespace TestAssignment.TestFileGenerator.Generators;

public sealed class StringGenerator : IStringGenerator
{
    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ";
    private const int MinStringLength = 1;
    private const int MaxStringLength = 250; //TODO make param of constructor
    private const int PercentOfRepeatingString = 20; //TODO make param of constructor
    private const int MaxRepeatingStrings = 1000; //TODO make param of constructor
    private readonly ConcurrentDictionary<int, string> repeatingStringsStore = new(); 
    private static readonly ThreadLocal<Random> _threadLocalRandom = new(() => new Random(Guid.NewGuid().GetHashCode()));

    public string Generate()
    {
        if (!repeatingStringsStore.IsEmpty && _threadLocalRandom.Value!.Next(0, 100) < PercentOfRepeatingString)
        {
            var index = _threadLocalRandom.Value!.Next(0, repeatingStringsStore.Count);
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
        int length = _threadLocalRandom.Value!.Next(MinStringLength, MaxStringLength + 1);
        char[] charArray = new char[length];
        for (int i = 0; i < length; i++)
        {
            charArray[i] = chars[_threadLocalRandom.Value!.Next(chars.Length)];
        }
        return new string(charArray);
    }
}
