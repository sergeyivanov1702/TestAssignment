namespace TestAssignment.Sorter.Interfaces;

public interface IChunkSorter
{
  string SortAndSave(List<LineRecord> chunk, string tempDirectory);
}
