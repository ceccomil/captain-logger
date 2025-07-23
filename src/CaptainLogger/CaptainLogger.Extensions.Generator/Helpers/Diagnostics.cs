namespace CaptainLogger.Extensions.Generator.Helpers;

internal static class Diagnostics
{
  public static void ReportWarning(
    this SourceProductionContext context,
    string id,
    string message,
    string category,
    Location? location = null)
  {
    context.ReportDiagnostic(
      Diagnostic.Create(
          new DiagnosticDescriptor(
              id,
              title: $"Warning ({id})",
              messageFormat: message,
              category,
              defaultSeverity: DiagnosticSeverity.Warning,
              isEnabledByDefault: true),
          location));
  }
}
