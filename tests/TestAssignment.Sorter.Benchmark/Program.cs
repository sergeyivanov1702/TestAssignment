using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;

var config = ManualConfig.Create(DefaultConfig.Instance)
    .AddDiagnoser(MemoryDiagnoser.Default)
    .AddDiagnoser(ThreadingDiagnoser.Default);

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
