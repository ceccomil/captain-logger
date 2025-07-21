namespace CaptainLogger.Helpers;

internal static class StreamsExtensions
{
  public static void CloseAndDispose(this Stream? stream)
  {
    if (stream is null)
    {
      return;
    }

    stream.Close();
    stream.Dispose();
  }

  public static void CloseAndDispose(this TextWriter? stream)
  {
    if (stream is null)
    {
      return;
    }

    stream.Close();
    stream.Dispose();
  }

  public static FileInfo InitAndLock(
    this FileInfo file,
    ref FileStream? fs,
    ref StreamWriter? sw)
  {

    fs = new FileStream(file.FullName, FileMode.OpenOrCreate, FileAccess.Write);
    fs.Position = fs.Length;
    sw = new StreamWriter(fs, Encoding.UTF8);

    return file;
  }
}
