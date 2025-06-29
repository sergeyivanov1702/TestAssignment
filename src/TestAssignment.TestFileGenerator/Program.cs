using System.CommandLine;
using System.Diagnostics;
using TestAssignment.TestFileGenerator.Generators;

var filePathArg = new Argument<string>(
    name: "filePath",
    description: "Path to the input file"
);

var minSizeArg = new Argument<long>(
    name: "targetMinimumSize",
    description: "Minimum size to apply"
);

var unitArg = new Argument<SizeUnit>(
    name: "sizeUnit",
    description: "Unit of size: MB, GB"
);

var rootCommand = new RootCommand("File Generator");
rootCommand.AddArgument(filePathArg);
rootCommand.AddArgument(minSizeArg);
rootCommand.AddArgument(unitArg);

rootCommand.SetHandler(async (filePath, targetMinimumSize, sizeUnit) =>
{
    try
    {
        Console.WriteLine("File generation process started.");
        Stopwatch stopwatch = Stopwatch.StartNew();

        long multiplier = sizeUnit switch
        {
            SizeUnit.MB => 1024L * 1024,
            SizeUnit.GB => 1024L * 1024 * 1024,
            _ => throw new ArgumentOutOfRangeException(nameof(sizeUnit), sizeUnit, "Unsupported size unit.")
        };

        long sizeInBytes = targetMinimumSize * multiplier;

        Console.WriteLine($"Target file: {filePath}");
        Console.WriteLine($"Target minimum size: {targetMinimumSize} {sizeUnit}");

        var stringGenerator = new StringGenerator();
        var lineGenerator = new LineGenerator(stringGenerator);
        var fileGenerator = new FileGenerator(lineGenerator);

        await fileGenerator.GenerateAsync(filePath, sizeInBytes);
        stopwatch.Stop();

        Console.WriteLine($"\nFile '{filePath}' generated successfully in {stopwatch.Elapsed.TotalSeconds} seconds.");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"\nAn error occurred during file generation: {ex.Message}");
    }
}, filePathArg, minSizeArg, unitArg);

return await rootCommand.InvokeAsync(args);

internal enum SizeUnit
{
    MB,
    GB
}
