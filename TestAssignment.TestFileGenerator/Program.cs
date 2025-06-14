using System.Diagnostics;
using TestAssignment.TestFileGenerator.Generators;

namespace TestAssignment.TestFileGenerator;

internal class Program
{
    static async Task Main(string[] args)
    {
        string outputFilePath = args[0];

        var stringGenerator = new StringGenerator();
        var lineGenerator = new LineGenerator(stringGenerator);
        var fileGenerator = new FileGeneratorNew(lineGenerator);

        var sw = new Stopwatch();
        sw.Start();

        await fileGenerator.GenerateAsync(outputFilePath, (long)500 * 1024 * 1024); // Generate a file of approximately 500MB

        sw.Stop();

        Console.WriteLine(sw.Elapsed.TotalSeconds);
    }
}
