
namespace TestAssignment.TestFileGenerator.Interfaces;

public interface IFileGenerator
{
    Task GenerateAsync(string outputFilePath, long targetSizeInBytes);
}