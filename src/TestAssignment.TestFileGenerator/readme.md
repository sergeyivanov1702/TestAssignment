# Test Assignment File Generator

This solution contains three projects:

1.  `TestAssignment.TestFileGenerator`: A console application to generate large files with each line following the format `<Number>. <String>`
2.  `TestAssignment.TestFileGenerator.Benchmark`: A project to benchmark the performance of the file generation process.
3.  `TestAssignment.TestFileGenerator.Tests`: Unit tests for the file generator.

## How to Run

### File Generator

The main application `TestAssignment.TestFileGenerator` can be run from the command line. It accepts three arguments:

-   `--output`: The path where the generated file will be saved.
-   `--size`: The minimum size of the generated file.
-   `--unit`: The unit of the size, which can be `MB` or `GB`.

**Example:**

```bash
dotnet run -c Release --project .\src\TestAssignment.TestFileGenerator\TestAssignment.TestFileGenerator.csproj -- --output "test.txt" --size 10 --unit GB
```	

This command will generate a file named `test.txt` with a minimum size of 10 GB.

### Benchmarks

To run the benchmarks, execute the following command:

```bash
dotnet run --project .\tests\TestAssignment.TestFileGenerator.Benchmark\TestAssignment.TestFileGenerator.Benchmark.csproj -c Release --filter **
```

The benchmark results will be saved in the `BenchmarkDotNet.Artifacts` directory.

### Tests

To run the unit tests, use the following command:

```bash
dotnet test
```
