namespace CaptainLogger.Helper;

internal static class DiExtensions
{
    public static IEnumerable<T> TryGetServices<T>(this IServiceProvider sp)
    {
        if (sp is null)
            throw new ArgumentNullException(nameof(sp));

         var services = sp.GetService<IEnumerable<T>>();

        if (services is null)
            return Enumerable.Empty<T>();

        return services;
    }
}
