using TestAssignment.Sorter.Interfaces;

namespace TestAssignment.Sorter;

public sealed class ChunkReader : IChunkReader
{
    public IEnumerable<List<LineRecord>> ReadChunks(string inputPath, long chunkSizeInBytes)
    {
        using var reader = new StreamReader(inputPath);
        while (!reader.EndOfStream)
        {
            var chunk = new List<LineRecord>();
            long totalBytes = 0;

            while (totalBytes < chunkSizeInBytes && !reader.EndOfStream)
            {
                string? line = reader.ReadLine();
                if (line == null) break;

                var record = LineRecord.Parse(line);
                chunk.Add(record);
                totalBytes += line.Length + Environment.NewLine.Length;
            }

            if (chunk.Count > 0)
                yield return chunk;
        }
    }
}
