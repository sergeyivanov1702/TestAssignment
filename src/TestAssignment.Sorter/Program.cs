using CommandLine;
using TestAssignment.Sorter;

var parser = new Parser(settings =>
{
    settings.IgnoreUnknownArguments = true;
    settings.HelpWriter = Console.Out;
});

parser.ParseArguments<SortOptions>(args)
    .WithParsed(SortFile);

static void SortFile(SortOptions opts)
{
    // Ensure temp directory exists
    Directory.CreateDirectory(opts.TempDirectory);

    var reader = new ChunkReader();
    var sorter = new ChunkSorter();
    var merger = new ChunkMerger();

    var fileSorter = new ExternalSorter(reader, sorter, merger);
    fileSorter.Execute(opts);
}