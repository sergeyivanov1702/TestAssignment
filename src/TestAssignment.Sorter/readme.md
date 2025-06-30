# Test Assignment Sorter

This solution contains three projects:

1.  `TestAssignment.Sorter`: A console application to sort large files where each line follows the format `<Number>. <String>`. It uses an external sort algorithm.
2.  `TestAssignment.Sorter.Benchmark`: A project to benchmark the performance of the file sorting process.
3.  `TestAssignment.Sorter.Tests`: Unit tests for the file sorter.

## How to Run

### File Sorter

The main application `TestAssignment.Sorter` can be run from the command line. It accepts the following arguments:

-   `--input` (`-i`): (Required) The path to the input file to be sorted.
-   `--output` (`-o`): (Required) The path where the sorted file will be saved.
-   `--temp-dir`: The directory for temporary chunk files. Defaults to the system's temp directory.
-   `--keep-temp`: If specified, temporary files will not be deleted after the sort is complete.
-   `--chunk-size`: The size of chunks to split the original file into, in bytes. Default is 100MB.
-   `--parallelism`: The maximum degree of parallelism to use. Defaults to the number of processor cores.

**Example:**

```bash
dotnet run --project .\src\TestAssignment.Sorter\TestAssignment.Sorter.csproj -- --input "test.txt" --output "sorted_test.txt"
```

This command will sort the file named `test.txt` and save the result to `sorted_test.txt`.

### Benchmarks

To run the benchmarks, execute the following command:

```bash
dotnet run --project .\tests\TestAssignment.Sorter.Benchmark\TestAssignment.Sorter.Benchmark.csproj -c Release --filter **
```

The benchmark results will be saved in the `BenchmarkDotNet.Artifacts` directory.

### Tests

To run the unit tests, use the following command:

```bash
dotnet test
```
