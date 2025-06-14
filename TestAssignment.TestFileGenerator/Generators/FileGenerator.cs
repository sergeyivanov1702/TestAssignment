using TestAssignment.TestFileGenerator.Interfaces;

namespace TestAssignment.TestFileGenerator.Generators;

public sealed class FileGenerator : IFileGenerator
{
    private readonly ILineGenerator _lineGenerator;

    public FileGenerator(ILineGenerator lineGenerator)
    {
        _lineGenerator = lineGenerator ?? throw new ArgumentNullException(nameof(lineGenerator));
    }

    public async Task GenerateAsync(string outputFilePath, long targetSizeInBytes)
    {
        if (string.IsNullOrWhiteSpace(outputFilePath))
            throw new ArgumentException("File path must not be null or empty.", nameof(outputFilePath));

        using var fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

        long currentSize = 0;
        while (currentSize < targetSizeInBytes)
        {
            var line = _lineGenerator.Generate();
            var bytes = System.Text.Encoding.UTF8.GetBytes(line + Environment.NewLine);
            await fileStream.WriteAsync(bytes);
            currentSize += bytes.Length;
        }

        await fileStream.FlushAsync();
    }
}
