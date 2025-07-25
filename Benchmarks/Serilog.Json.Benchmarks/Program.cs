using LogsBenchmark;

if (Directory.Exists(Constants.LOG_FOLDER))
{
  Directory.Delete(Constants.LOG_FOLDER, recursive: true);
}

Directory.CreateDirectory(Constants.LOG_FOLDER);

#if DEBUG

var benchmark = new LogsServiceBenchmark();
benchmark.Setup();
benchmark.LogsInParallel();

#else

BenchmarkDotNet.Running.BenchmarkRunner.Run<LogsServiceBenchmark>();

#endif
