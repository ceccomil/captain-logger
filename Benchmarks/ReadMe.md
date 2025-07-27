```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4770/24H2/2024Update/HudsonValley)
QEMU Virtual CPU version 2.5+ 3.40GHz, 2 CPU, 24 logical and 24 physical cores
.NET SDK 9.0.302
  [Host]     : .NET 9.0.7 (9.0.725.31616), X64 RyuJIT SSE4.2
  DefaultJob : .NET 9.0.7 (9.0.725.31616), X64 RyuJIT SSE4.2


```
# Benchmark for Pure Captain Logger (CaptainLogger.Benchmarks)

| Method             | Mean                 | Error              | StdDev              | Gen0      | Allocated  |
|------------------- |---------------------:|-------------------:|--------------------:|----------:|-----------:|
| LogsInParallel     | 1,645,899,617.000 ns | 53,201,791.2164 ns | 156,866,693.1449 ns | 1000.0000 | 15868528 B |
| OneLogLine         |       219,118.998 ns |      9,426.9429 ns |      27,795.5559 ns |         - |     1561 B |
| OneLogLineDisabled |             7.448 ns |          0.0527 ns |           0.0493 ns |    0.0038 |       32 B |


# Benchmark for High Perf Captain Logger (CaptainLogger.Json.Benchmarks)

| Method             | Mean               | Error              | StdDev             | Gen0      | Allocated  |
|------------------- |-------------------:|-------------------:|-------------------:|----------:|-----------:|
| LogsInParallel     | 872,719,189.000 ns | 25,121,807.7976 ns | 74,072,222.4745 ns | 4000.0000 | 38085888 B |
| OneLogLine         |     142,832.169 ns |      4,802.4906 ns |     14,160.2529 ns |    0.4883 |     5819 B |
| OneLogLineDisabled |           7.387 ns |          0.0575 ns |          0.0481 ns |    0.0038 |       32 B |

# Benchmark for Serilog Logger (Serilog.Benchmarks)

| Method             | Mean              | Error             | StdDev            | Gen0      | Allocated  |
|------------------- |------------------:|------------------:|------------------:|----------:|-----------:|
| LogsInParallel     | 973,834,519.00 ns | 27,220,598.092 ns | 80,260,553.461 ns | 2000.0000 | 19433952 B |
| OneLogLine         |     115,612.55 ns |      4,032.862 ns |     11,890.986 ns |    0.1221 |     2001 B |
| OneLogLineDisabled |          10.89 ns |          0.094 ns |          0.083 ns |    0.0038 |       32 B |

# Benchmark for Serilog Json Logger (Serilog.Json.Benchmarks)


| Method             | Mean              | Error             | StdDev            | Gen0      | Allocated  |
|------------------- |------------------:|------------------:|------------------:|----------:|-----------:|
| LogsInParallel     | 907,951,997.00 ns | 25,862,899.354 ns | 76,257,347.809 ns | 3000.0000 | 25633312 B |
| OneLogLine         |     172,775.02 ns |      4,654.547 ns |     13,724.038 ns |    0.2441 |     3491 B |
| OneLogLineDisabled |          10.95 ns |          0.076 ns |          0.071 ns |    0.0038 |       32 B |
