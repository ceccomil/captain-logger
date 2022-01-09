namespace CaptainLogger.Tests;

public class ConcurrentLoggingTests
{
    private const string TESTS_LOGS = "./TestLogs";

    [Fact(DisplayName = "Each application instance should have a unique logfile")]
    public async Task FiveApplicationInstances()
    {
        var unqName = Guid.NewGuid().ToString();
        var logName = $"{TESTS_LOGS}/{unqName}.log";
        var tasks = new List<Task>();
        var guids = GetGuids();

        //Runs 5 applications in parallel
        for (int i = 0; i < 5; i++)
            tasks.Add(RunConsoleApp(guids[i], logName));

        await Task.WhenAll(tasks);

        var files = Directory
            .GetFiles(TESTS_LOGS, $"{unqName}*.log");

        Assert.Equal(5, files.Length);

        foreach (var file in files)
            File.Delete(file);
    }

    private static string[] GetGuids() => new string[5]
    {
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString()
    };

    private static async Task<(StringBuilder, StringBuilder)> RunConsoleApp(
        string instanceId,
        string logname)
    {
        using var proc = new Process();

        var output = new StringBuilder();
        var error = new StringBuilder();

        proc.StartInfo = new ProcessStartInfo("dotnet",
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                $"CaptainLogger.Tests.ConsoleApp.dll {instanceId} {logname}"))
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is not null)
                output.AppendLine(e.Data);
        }

        void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is not null)
                error.AppendLine(e.Data);
        }

        proc.OutputDataReceived += OutputDataReceived;
        proc.ErrorDataReceived += ErrorDataReceived;

        proc.Start();
        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();

        await proc.WaitForExitAsync();

        proc.OutputDataReceived -= OutputDataReceived;
        proc.ErrorDataReceived -= ErrorDataReceived;

        return (output, error);
    }
}