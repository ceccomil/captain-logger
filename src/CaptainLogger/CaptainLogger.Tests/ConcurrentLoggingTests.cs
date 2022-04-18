namespace CaptainLogger.Tests;

public class ConcurrentLoggingTests
{
    private const string TESTS_LOGS = "./TestLogs";

    [Fact(DisplayName = "(5 Instances) Each application instance should have a unique logfile")]
    public async Task FiveApplicationInstances()
    {
        var unqName = Guid.NewGuid().ToString();
        var tasks = new List<Task>();
        var guids = GetGuids();

        //Runs 5 applications in parallel
        for (var i = 0; i < 5; i++)
        {
            tasks.Add(RunConsoleApp(guids[i], GetLogFilePath(unqName)));
        }

        await Task.WhenAll(tasks);
        Assert.Equal(5, await CountAndCleanLogs(unqName));
    }

    [Fact(DisplayName = "(2 Instances) Application instances do not share logfiles")]
    public async Task ApplicationInstancesDontShare()
    {
        var unqName = Guid.NewGuid().ToString();
        var tasks = new List<Task>();
        var guids = GetGuids().Take(2).ToArray();

        //Runs 2 applications in parallel
        for (var i = 0; i < 2; i++)
        {
            tasks.Add(RunConsoleApp(guids[i], GetLogFilePath(unqName)));
        }

        await Task.WhenAll(tasks);
        //Check contents
        Assert.Equal(2, await CountAndCleanLogs(unqName, guids));
    }

    private static async Task<int> CountAndCleanLogs(string uniqueId, string[]? guids = default)
    {
        var logDir = new DirectoryInfo(GetLogsDir(uniqueId));
        var files = logDir.GetFiles($"*.log");

        if (guids is not null)
        {
            await CheckContents(files, guids);
        }

        logDir.Delete(recursive: true);
        return files.Length;
    }

    private static async Task CheckContents(FileInfo[] files, string[] guids)
    {
        if (files.Length != 2 || guids.Length != 2)
        {
            throw new ApplicationException($"CheckContents can be called with 2 files only!");
        }

        var content1 = await File.ReadAllTextAsync(files[0].FullName);
        var content2 = await File.ReadAllTextAsync(files[1].FullName);

        if (content1.Contains(guids[0]))
        {
            Assert.DoesNotContain(guids[0], content2);
            Assert.Contains(guids[1], content2);
        }
        else if (content1.Contains(guids[1]))
        {
            Assert.DoesNotContain(guids[1], content2);
            Assert.Contains(guids[0], content2);
        }
        else
        {
            throw new ApplicationException("At least one condition must be true!");
        }
    }

    private static string GetLogFilePath(string uniqueId) => $"{GetLogsDir(uniqueId)}/{AppDomain.CurrentDomain.FriendlyName}.log";
    private static string GetLogsDir(string uniqueId) => $"{TESTS_LOGS}/{uniqueId}";
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
            {
                output.AppendLine(e.Data);
            }
        }

        void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is not null)
            {
                error.AppendLine(e.Data);
            }
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