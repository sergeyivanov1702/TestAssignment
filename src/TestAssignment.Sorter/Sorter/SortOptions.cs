using CommandLine;
using System.Diagnostics.CodeAnalysis;

namespace TestAssignment.Sorter;

[ExcludeFromCodeCoverage]
public sealed class SortOptions
{
    [Option('i', "input", Required = true, HelpText = "Path to the input file.")]
    public string InputPath { get; set; } = string.Empty;

    [Option('o', "output", Required = true, HelpText = "Path to the output file.")]
    public string OutputPath { get; set; } = string.Empty;

    [Option("temp-dir", HelpText = "Temporary directory path.")]
    public string TempDirectory { get; set; } = Path.GetTempPath();

    [Option("keep-temp", Default = false, HelpText = "Keep temporary files after sorting is completed.")]
    public bool KeepTempFiles { get; set; }

    [Option("chunk-size", Default = 100L * 1024 * 1024, HelpText = "Chunk size in bytes.")]
    public long ChunkSizeInBytes { get; set; }

    [Option("parallelism", HelpText = "Max degree of parallelism.")]
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
}
