namespace CaptainLogger.Helpers;
internal static class StringBuilderCache
{
  // Builders bigger than this are not cached
  private const int MAX_BUILDER_SIZE = 2048;

  // One builder per thread – avoids locking.
  [ThreadStatic]
  private static StringBuilder? _cachedInstance;

  public static StringBuilder GetNewOrCached(int capacity = 16)
  {
    if (capacity <= MAX_BUILDER_SIZE)
    {
      var sb = _cachedInstance;
      if (sb is not null && capacity <= sb.Capacity)
      {
        _cachedInstance = null;
        sb.Length = 0;
        return sb;
      }
    }

    // fallback: allocate a fresh one
    return new StringBuilder(capacity);
  }

  public static void StoreForReuse(StringBuilder sb)
  {
    if (sb.Capacity <= MAX_BUILDER_SIZE)
    {
      _cachedInstance = sb;
    }
  }

  public static string GetStringAndCacheIt(StringBuilder sb)
  {
    string result = sb.ToString();
    StoreForReuse(sb);
    return result;
  }
}
