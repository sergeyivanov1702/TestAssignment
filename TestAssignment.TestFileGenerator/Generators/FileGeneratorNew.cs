using System.Buffers;
using System.Diagnostics;
using System.Text;
using System.Threading.Channels;
using TestAssignment.TestFileGenerator.Interfaces;

namespace TestAssignment.TestFileGenerator.Generators;
public sealed class FileGeneratorNew : IFileGenerator
{
    const int FlushLineCount = 5000;
    private readonly ILineGenerator _lineGenerator;

    public FileGeneratorNew(ILineGenerator lineGenerator)
    {
        _lineGenerator = lineGenerator ?? throw new ArgumentNullException(nameof(lineGenerator));
    }

    public async Task GenerateAsync(string outputFilePath, long targetSizeInBytes)
    {
        if (string.IsNullOrWhiteSpace(outputFilePath))
            throw new ArgumentException("File path must not be null or empty.", nameof(outputFilePath));

        int producerCount = Math.Max(1, Environment.ProcessorCount - 1);
        long totalBytesEmitted = 0L;

        // Bounded channel carrying (buffer, validLength) tuples
        var channel = Channel.CreateBounded<(byte[] Buffer, int Length)>(
            new BoundedChannelOptions(producerCount * 2)
            {
                SingleWriter = false,
                SingleReader = true,
                FullMode = BoundedChannelFullMode.Wait
            });

        // Spawn producers
        var producers = new List<Task>();
        for (int i = 0; i < producerCount; i++)
        {
            producers.Add(Task.Run(async () =>
            {
                var sb = new StringBuilder(FlushLineCount * 200);
                while (true)
                {
                    sb.Clear();
                    for (int j = 0; j < FlushLineCount; j++)
                    {
                        string line = _lineGenerator.Generate();
                        sb.AppendLine(line);
                    }

                    string blockText = sb.ToString();
                    int byteCount = Encoding.UTF8.GetByteCount(blockText);

                    // Atomically track progress and bail out if we've hit target
                    long newTotal = Interlocked.Add(ref totalBytesEmitted, byteCount);
                    if (newTotal > targetSizeInBytes)
                    {
                        // undo overshoot and stop
                        Interlocked.Add(ref totalBytesEmitted, -byteCount);
                        break;
                    }

                    // Rent a buffer, encode, and enqueue
                    byte[] buffer = ArrayPool<byte>.Shared.Rent(byteCount);
                    int actualLength = Encoding.UTF8.GetBytes(blockText, 0, blockText.Length, buffer, 0);
                    await channel.Writer.WriteAsync((buffer, actualLength));
                }
            }));
        }

        var consumer = Task.Run(async () =>
        {
            await using var fs = new FileStream(
                outputFilePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 1 << 20,    // 1 MB OS buffer
                useAsync: true);

            await foreach (var (buffer, length) in channel.Reader.ReadAllAsync())
            {
                await fs.WriteAsync(buffer.AsMemory(0, length));
                ArrayPool<byte>.Shared.Return(buffer);
            }
        });

        // Wait producers, then complete the channel and await the consumer
        await Task.WhenAll(producers);
        channel.Writer.Complete();
        await consumer;
    }
}
