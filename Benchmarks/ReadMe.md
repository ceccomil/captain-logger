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
| LogsInParallel     | 1,570,939,789.000 ns | 49,909,144.1685 ns | 147,158,248.3295 ns | 1000.0000 | 15480112 B |
| OneLogLine         |       227,733.380 ns |      7,723.0069 ns |      22,771.4617 ns |         - |     1481 B |
| OneLogLineDisabled |             7.570 ns |          0.0285 ns |           0.0238 ns |    0.0038 |       32 B |


# Benchmark for High Perf Captain Logger (CaptainLogger.Json.Benchmarks)

| Method             | Mean              | Error             | StdDev            | Gen0      | Allocated  |
|------------------- |------------------:|------------------:|------------------:|----------:|-----------:|
| LogsInParallel     | 907,502,510.00 ns | 25,740,718.180 ns | 75,897,093.834 ns | 3000.0000 | 25620976 B |
| OneLogLine         |     165,528.34 ns |      4,498.094 ns |     13,262.733 ns |    0.2441 |     3491 B |
| OneLogLineDisabled |          10.78 ns |          0.164 ns |          0.154 ns |    0.0038 |       32 B |

# Benchmark for Serilog Logger (Serilog.Benchmarks)

| Method             | Mean              | Error             | StdDev            | Gen0      | Allocated  |
|------------------- |------------------:|------------------:|------------------:|----------:|-----------:|
| LogsInParallel     | 960,731,690.91 ns | 30,477,047.096 ns | 89,383,909.852 ns | 2000.0000 | 19420576 B |
| OneLogLine         |     123,295.74 ns |      4,232.458 ns |     12,479.499 ns |         - |     2001 B |
| OneLogLineDisabled |          10.87 ns |          0.077 ns |          0.072 ns |    0.0038 |       32 B |

# Benchmark for Serilog Json Logger (Serilog.Json.Benchmarks)


| Method             | Mean              | Error             | StdDev            | Gen0      | Allocated  |
|------------------- |------------------:|------------------:|------------------:|----------:|-----------:|
| LogsInParallel     | 907,502,510.00 ns | 25,740,718.180 ns | 75,897,093.834 ns | 3000.0000 | 25620976 B |
| OneLogLine         |     165,528.34 ns |      4,498.094 ns |     13,262.733 ns |    0.2441 |     3491 B |
| OneLogLineDisabled |          10.78 ns |          0.164 ns |          0.154 ns |    0.0038 |       32 B |
