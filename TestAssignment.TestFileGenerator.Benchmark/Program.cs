using BenchmarkDotNet.Running;
using TestAssignment.TestFileGenerator.Benchmark;

var summary = BenchmarkRunner.Run<LineGeneratorBenchmarks>();