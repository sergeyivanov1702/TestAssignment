using TestAssignment.Sorter.Interfaces;

namespace TestAssignment.Sorter;

public sealed class ChunkMerger : IChunkMerger
{
    public void Merge(IEnumerable<string> sortedTempPaths, string outputPath)
    {
        var readers = new List<StreamReader>();
        var heap = new PriorityQueue<HeapNode, LineRecord>();

        try
        {
            foreach (var path in sortedTempPaths)
            {
                var reader = new StreamReader(path);
                readers.Add(reader);
                if (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line != null)
                    {
                        var rec = LineRecord.Parse(line);
                        heap.Enqueue(new HeapNode(rec, reader), rec);
                    }
                }
            }

            using var writer = new StreamWriter(outputPath, false, System.Text.Encoding.UTF8, 1 << 20);

            while (heap.Count > 0)
            {
                var node = heap.Dequeue();
                writer.WriteLine(node.Record.OriginalLine);

                if (!node.Reader.EndOfStream)
                {
                    var nextLine = node.Reader.ReadLine();
                    if (nextLine != null)
                    {
                        var nextRec = LineRecord.Parse(nextLine);
                        heap.Enqueue(new HeapNode(nextRec, node.Reader), nextRec);
                    }
                }
                else
                {
                    node.Reader.Dispose();
                }
            }
        }
        finally
        {
            foreach (var reader in readers)
                reader.Dispose();
        }
    }

    private readonly struct HeapNode
    {
        public LineRecord Record { get; }
        public StreamReader Reader { get; }

        public HeapNode(LineRecord record, StreamReader reader)
        {
            Record = record;
            Reader = reader;
        }
    }
}
