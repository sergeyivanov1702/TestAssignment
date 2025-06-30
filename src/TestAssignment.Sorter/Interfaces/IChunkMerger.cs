namespace TestAssignment.Sorter.Interfaces;

public interface IChunkMerger
{
  void Merge(IEnumerable<string> sortedTempPaths, string outputPath);
}
