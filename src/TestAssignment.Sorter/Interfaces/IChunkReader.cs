namespace TestAssignment.Sorter.Interfaces;

public interface IChunkReader
{
  IEnumerable<List<LineRecord>> ReadChunks(string inputPath, long chunkSizeInBytes);
}
