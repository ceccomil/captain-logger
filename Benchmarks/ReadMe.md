﻿```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4770/24H2/2024Update/HudsonValley)
QEMU Virtual CPU version 2.5+ 3.40GHz, 2 CPU, 24 logical and 24 physical cores
.NET SDK 9.0.302
  [Host]     : .NET 9.0.7 (9.0.725.31616), X64 RyuJIT SSE4.2
  DefaultJob : .NET 9.0.7 (9.0.725.31616), X64 RyuJIT SSE4.2


```
# Benchmark for Pure Captain Logger (CaptainLogger.Benchmarks)

| Method             | Mean                 | Error              | StdDev              | Gen0      | Allocated  |
|------------------- |---------------------:|-------------------:|--------------------:|----------:|-----------:|
| LogsInParallel     | 1,648,010,434.000 ns | 44,974,211.9105 ns | 132,607,488.1670 ns | 2000.0000 | 18549984 B |
| OneLogLine         |       230,109.797 ns |      8,724.7247 ns |      25,725.0494 ns |         - |     2058 B |
| OneLogLineDisabled |             7.432 ns |          0.0508 ns |           0.0476 ns |    0.0038 |       32 B |


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
