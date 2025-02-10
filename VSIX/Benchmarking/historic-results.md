# Rapid XAML Analysis - Benchmarking - Historic results

Because any changes can be far reaching, it is not practical to keep all different implementations for comparison.
Instead, a copy of the results is kept for comparison with current results.

A benchmarking run involves parsing the five documents in the `files` folder.
This includes analysis with all default/included analyzers and so performance may suffer as more analyzers are included. However, the ability to monitor this will help identify the impact of adding new analyzers.

Run should be done of a release build without the debugger attached.

If running this on your own machine, run the test against the default branch first to see relative change/differences.

Add new results below here but above old results.

## post 0.11.4 (perf review - 6 Oct 2020)

```ascii
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.20175
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.100-preview.8.20417.9
  [Host]     : .NET Core 3.1.8 (CoreCLR 4.700.20.41105, CoreFX 4.700.20.41903), X64 RyuJIT
  DefaultJob : .NET Core 3.1.8 (CoreCLR 4.700.20.41105, CoreFX 4.700.20.41903), X64 RyuJIT

|       Method |     Mean |    Error |   StdDev | Ratio | Rank |     Gen 0 |     Gen 1 |    Gen 2 | Allocated |
|------------- |---------:|---------:|---------:|------:|-----:|----------:|----------:|---------:|----------:|
| ParseCurrent | 62.49 ms | 0.898 ms | 0.701 ms |  1.00 |    1 | 8000.0000 | 1125.0000 | 500.0000 |   33.9 MB |
```

## 0.10.5 (initial perf improvements)

Summary results

```ascii
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.20175
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.100-preview.6.20318.15
  [Host]     : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT
  DefaultJob : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT

|       Method |     Mean |    Error |   StdDev | Ratio | Rank |      Gen 0 |     Gen 1 | Gen 2 | Allocated |
|------------- |---------:|---------:|---------:|------:|-----:|-----------:|----------:|------:|----------:|
| ParseCurrent | 230.9 ms | 12.24 ms | 34.53 ms |  1.00 |    1 | 11000.0000 | 2000.0000 |     - |  47.93 MB |
```

## 0.10.2 (initial benchmark)

Summary results

```ascii
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19628
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.100-preview.6.20318.15
  [Host]     : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT
  DefaultJob : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT

|       Method |     Mean |    Error |   StdDev | Ratio | Rank |      Gen 0 |     Gen 1 | Gen 2 | Allocated |
|------------- |---------:|---------:|---------:|------:|-----:|-----------:|----------:|------:|----------:|
| ParseCurrent | 426.9 ms | 18.65 ms | 53.51 ms |  1.00 |    1 | 18000.0000 | 2000.0000 |     - |  79.86 MB |
```

**Notes.**

- Mean is better than expected.
- Allocated memory is worryingly large.
