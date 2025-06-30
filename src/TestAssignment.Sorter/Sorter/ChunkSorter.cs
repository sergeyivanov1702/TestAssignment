using TestAssignment.Sorter.Interfaces;

namespace TestAssignment.Sorter;

public sealed class ChunkSorter : IChunkSorter
{
    public string SortAndSave(List<LineRecord> chunk, string tempDirectory)
    {
        chunk.Sort();

        string tempPath = Path.Combine(tempDirectory, Guid.NewGuid() + ".tmp");
        using (var writer = new StreamWriter(tempPath, false, System.Text.Encoding.UTF8, 1 << 20))
        {
            foreach (var record in chunk)
            {
                writer.WriteLine(record.OriginalLine);
            }
        }

        return tempPath;
    }
}
