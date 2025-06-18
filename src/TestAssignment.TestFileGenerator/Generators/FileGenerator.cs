using System.Buffers;
using System.Text;
using System.Threading.Channels;
using TestAssignment.TestFileGenerator.Interfaces;

namespace TestAssignment.TestFileGenerator.Generators;

public sealed class FileGenerator : IFileGenerator
{
    private const int FlushLineCount = 7000;

    private static readonly int ProducersCount = Math.Max(1, Environment.ProcessorCount - 1);

    private readonly ILineGenerator _lineGenerator;
    private long _totalBytesEmitted;

    public FileGenerator(ILineGenerator lineGenerator)
    {
        _lineGenerator = lineGenerator ?? throw new ArgumentNullException(nameof(lineGenerator));
    }

    public async Task GenerateAsync(string outputFilePath, long targetSizeInBytes)
    {
        if (string.IsNullOrWhiteSpace(outputFilePath))
            throw new ArgumentException("File path must not be null or empty.", nameof(outputFilePath));

        if (targetSizeInBytes == 0)
            throw new ArgumentException("Target size in bytes must be greater than 0", nameof(targetSizeInBytes));

        _totalBytesEmitted = 0L;

        var channel = Channel.CreateBounded<(byte[] Buffer, int Length)>(
            new BoundedChannelOptions(ProducersCount * 2)
            {
                SingleWriter = false,
                SingleReader = true,
                FullMode = BoundedChannelFullMode.Wait
            });

        var producers = new List<Task>();
        for (int i = 0; i < ProducersCount; i++)
        {
            producers.Add(Task.Run(() => GenerateDataAsync(channel.Writer, targetSizeInBytes)));
        }

        var consumer = Task.Run(() => WriteDataToFileAsync(channel.Reader, outputFilePath));

        await Task.WhenAll(producers);
        channel.Writer.Complete();
        await consumer;
    }

    private async Task GenerateDataAsync(
        ChannelWriter<(byte[] Buffer, int Length)> writer,
        long targetMinimumSizeInBytes)
    {
        while (true)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(1 << 20); // Rent 1 MB buffer
            int offset = 0;

            for (int j = 0; j < FlushLineCount; j++)
            {
                byte[] lineBytes = _lineGenerator.Generate();
                // Resize buffer if needed
                if (offset + lineBytes.Length > buffer.Length)
                {
                    byte[] newBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length * 2);
                    Buffer.BlockCopy(buffer, 0, newBuffer, 0, offset);
                    ArrayPool<byte>.Shared.Return(buffer);
                    buffer = newBuffer;
                }

                Buffer.BlockCopy(lineBytes, 0, buffer, offset, lineBytes.Length);
                offset += lineBytes.Length;
            }

            var newTotal = Interlocked.Add(ref _totalBytesEmitted, offset);
            await writer.WriteAsync((buffer, offset));

            // total file will be at least of targetMinimumSizeInBytes
            if (newTotal >= targetMinimumSizeInBytes)
            {
                break;
            }
        }
    }

    private static async Task WriteDataToFileAsync(ChannelReader<(byte[] Buffer, int Length)> reader, string outputFilePath)
    {
        await using var fileStream = new FileStream(
            outputFilePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 1 << 20, // 1 MB OS buffer
            useAsync: true);

        await foreach (var (buffer, length) in reader.ReadAllAsync())
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, length));
            ArrayPool<byte>.Shared.Return(buffer);
        }

        var newlineLength = Encoding.UTF8.GetBytes(Environment.NewLine).Length;
        if (fileStream.Length >= newlineLength)
        {
            fileStream.SetLength(fileStream.Length - newlineLength);
        }
    }
}
