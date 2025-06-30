using System.Diagnostics;
using CommandLine;
using TestAssignment.TestFileGenerator;

var parser = new Parser(settings =>
{
    settings.IgnoreUnknownArguments = true;
    settings.HelpWriter = Console.Out;
});

parser.ParseArguments<Options>(args)
    .WithParsed(Run);

static void Run(Options opts)
{
    try
    {
        Console.WriteLine("File generation process started.");

        long multiplier = opts.SizeUnit switch
        {
            SizeUnit.MB => 1024L * 1024,
            SizeUnit.GB => 1024L * 1024 * 1024,
            _ => throw new ArgumentOutOfRangeException(nameof(opts.SizeUnit), opts.SizeUnit, "Unsupported size unit.")
        };

        long sizeInBytes = opts.TargetMinimumSize * multiplier;

        Console.WriteLine($"Target file: {opts.FilePath}");
        Console.WriteLine($"Target minimum size: {opts.TargetMinimumSize} {opts.SizeUnit}");

        var stopwatch = Stopwatch.StartNew();

        var stringGenerator = new StringGenerator();
        var numberGenerator = new NumberGenerator();
        var lineGenerator = new LineGenerator(stringGenerator, numberGenerator);
        var fileGenerator = new FileGenerator(lineGenerator);

        fileGenerator.GenerateAsync(opts.FilePath, sizeInBytes).GetAwaiter().GetResult();
        stopwatch.Stop();

        Console.WriteLine($"\nFile '{opts.FilePath}' generated successfully in {stopwatch.Elapsed.TotalSeconds} seconds.");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"\nAn error occurred during file generation: {ex.Message}");
    }
}

internal class Options
{
    [Option("output", Required = true, HelpText = "Path to the output file")]
    public string FilePath { get; set; } = null!;

    [Option("size", Required = true, HelpText = "Minimum size to apply")]
    public long TargetMinimumSize { get; set; }

    [Option("unit", Required = true, HelpText = "Unit of size: MB, GB")]
    public SizeUnit SizeUnit { get; set; }
}

internal enum SizeUnit
{
    MB,
    GB
}
