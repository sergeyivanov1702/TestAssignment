using CommandLine;
using TestAssignment.Sorter;

var parser = new Parser(settings =>
{
    settings.IgnoreUnknownArguments = true;
});

parser.ParseArguments<SortOptions>(args)
    .WithParsed(SortFile)
    .WithNotParsed(HandleParseError);

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

static void HandleParseError(IEnumerable<Error> errs)
{
    var actualErrors = errs.Where(e => e.Tag != ErrorType.UnknownOptionError);
    foreach (var error in actualErrors)
    {
        Console.Error.WriteLine(error.ToString());
    }
}
