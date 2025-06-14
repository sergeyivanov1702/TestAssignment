using System.Collections.Concurrent;
using TestAssignment.TestFileGenerator.Interfaces;

namespace TestAssignment.TestFileGenerator.Generators;
public sealed class ByteArrayGenerator : IByteArrayGenerator
{
    private const int MinArrayLength = 1;
    private const int MaxArrayLength = 250; //TODO make param of constructor
    private const int PercentOfRepeatingArray = 20; //TODO make param of constructor
    private const int MaxRepeatingArrays = 1000; //TODO make param of constructor
    private readonly ConcurrentDictionary<int, byte[]> repeatingArraysStore = new();
    private static readonly ThreadLocal<Random> _threadLocalRandom = new(() => new Random(Guid.NewGuid().GetHashCode()));

    public byte[] Generate()
    {
        var random = _threadLocalRandom.Value!;
        if (!repeatingArraysStore.IsEmpty && random.Next(0, 100) < PercentOfRepeatingArray)
        {
            var index = random.Next(0, repeatingArraysStore.Count);
            if (repeatingArraysStore.TryGetValue(index, out var existingArray))
            {
                return existingArray;
            }
        }

        var generatedArray = GenerateRandomByteArray(random);
        // TODO For now I don't care if it adds more items than MaxRepeatingArrays
        if (repeatingArraysStore.Count < MaxRepeatingArrays)
        {
            repeatingArraysStore.AddOrUpdate(repeatingArraysStore.Count, generatedArray, (_, oldValue) => oldValue);
        }

        return generatedArray;
    }

    private static byte[] GenerateRandomByteArray(Random random)
    {
        int length = random.Next(MinArrayLength, MaxArrayLength + 1);
        byte[] byteArray = new byte[length];
        random.NextBytes(byteArray);
        return byteArray;
    }
}