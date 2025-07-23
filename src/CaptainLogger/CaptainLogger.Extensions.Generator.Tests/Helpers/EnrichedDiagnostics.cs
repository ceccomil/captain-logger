namespace CaptainLogger.Extensions.Generator.Tests.Helpers;

internal sealed class EnrichedDiagnostics
{
  private ImmutableArray<Diagnostic> Diagnostics { get; }

  public IEnumerable<Diagnostic> Errors => GetErrors(Diagnostics);
  public IEnumerable<Diagnostic> Warnings => GetWarnings(Diagnostics);
  public IEnumerable<Diagnostic> Info => GetInfo(Diagnostics);

  public EnrichedDiagnostics(ImmutableArray<Diagnostic> diagnostics)
  {
    Diagnostics = diagnostics;
  }

  public EnrichedDiagnostics(Compilation compilation)
  {
    Diagnostics = compilation.GetDiagnostics();
  }

  private IEnumerable<string> ErrorArguments => GetErrorArguments();

  public string ReadableErrors => GetReadableErrors();

  public string ReadableWarnings => GetReadableWarnings();

  public string ReadableInfo => GetReadableInfo();

  private static IEnumerable<Diagnostic> GetErrors(
    ImmutableArray<Diagnostic> diagnostics) => diagnostics
      .Where(x => x.Severity == DiagnosticSeverity.Error);

  private static IEnumerable<Diagnostic> GetWarnings(
    ImmutableArray<Diagnostic> diagnostics) => diagnostics
      .Where(x => x.Severity == DiagnosticSeverity.Warning);

  private static IEnumerable<Diagnostic> GetInfo(
    ImmutableArray<Diagnostic> diagnostics) => diagnostics
      .Where(x => (int)x.Severity <= 1);

  private static string GetReadable(IEnumerable<Diagnostic> diagnostics)
  {
    var texts = diagnostics
      .Select(x => x.ToString());

    return string.Join(Environment.NewLine, texts);
  }

  private string GetReadableErrors() => GetReadable(Errors);

  private string GetReadableWarnings() => GetReadable(Warnings);

  private string GetReadableInfo() => GetReadable(Info);

  private IEnumerable<string> GetErrorArguments()
  {
    List<string> args = [];

    foreach (var error in Errors)
    {
      try
      {
        var pi = error
          .GetType()
          .GetProperty("Arguments", BindingFlags.NonPublic | BindingFlags.Instance)!;

        var objArgs = pi
          .GetValue(error) as IReadOnlyList<object>;

        args.AddRange(objArgs!.Select(x => x.ToString()!));
      }
      catch
      {
        // do nothing!
      }
    }

    return args.Distinct();
  }

  public override string ToString()
  {
    return
      $"Debugging information: {ErrorArguments.Count()}" +
      $" arguments, Errors: {ReadableErrors} --- Warnings: {ReadableWarnings}" +
      $" --- Info: {ReadableInfo}";
  }
}
