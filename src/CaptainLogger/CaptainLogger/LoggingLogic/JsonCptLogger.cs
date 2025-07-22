namespace CaptainLogger.LoggingLogic;

internal class JsonCptLogger(
  string name,
  Func<CaptainLoggerOptions> getCurrentConfig)
  : CptLoggerBase(name, getCurrentConfig)
{
  private const string ORIGINAL_FORMAT = "{OriginalFormat}";

  protected override async Task WriteLog<TState>(
    DateTime time,
    CaptainLoggerOptions config,
    LogLevel level,
    TState state,
    EventId eventId,
    Exception? ex,
    Func<TState, Exception?, string> formatter)
  {
    var line = GetLogLine(
      time,
      state,
      eventId,
      level,
      ex,
      config.DoNotAppendException,
      formatter);

    if (config.LogRecipients.HasFlag(Recipients.Console))
    {
      Console.WriteLine(Encoding.UTF8.GetString(line.WrittenSpan));
    }

    if (config.LogRecipients.HasFlag(Recipients.File))
    {
      await line.WriteToLogFile(time, config);
    }

    if (config.LogRecipients.HasFlag(Recipients.Stream))
    {
      await WriteToBuffer(line, config);
    }
  }

  private static async Task WriteToBuffer(
    ArrayBufferWriter<byte> line,
    CaptainLoggerOptions config)
  {
    if (config.LoggerBuffer is null)
    {
      throw new InvalidOperationException(
        "Log Buffer stream must be a valid opened `System.Stream`!");
    }

    await config.LoggerBuffer.WriteAsync(line.WrittenMemory);
    config.LoggerBuffer.Flush();
  }

  private ArrayBufferWriter<byte> GetLogLine<TState>(
    DateTime time,
    TState state,
    EventId eventId,
    LogLevel level,
    Exception? ex,
    bool doNotAppendEx,
    Func<TState, Exception?, string> formatter)
  {
    var buffer = new ArrayBufferWriter<byte>();
    using var writer = new Utf8JsonWriter(buffer);

    writer.WriteStartObject();
    writer.WriteString("timestamp", time);
    writer.WriteString("level", level.ToCaptainLoggerString());

    writer.WriteStartObject("event");
    writer.WriteNumber("id", eventId.Id);

    if (!string.IsNullOrWhiteSpace(eventId.Name))
    {
      writer.WriteString("name", eventId.Name);
    }

    writer.WriteEndObject();

    writer.WriteString("category", Category);

    writer.WriteStartObject("content");

    WriteContent(writer, state, formatter);

    writer.WriteEndObject();

    if (!doNotAppendEx && ex is not null)
    {
      writer.WriteStartObject("exception");
      writer.WriteString("message", ex.Message);
      writer.WriteString("stackTrace", ex.StackTrace);
      writer.WriteEndObject();
    }

    writer.WriteEndObject();

    writer.Flush();

    return buffer;
  }

  private static void WriteContent<TState>(
    Utf8JsonWriter writer,
    TState state,
    Func<TState, Exception?, string> formatter)
  {
    if (state is null)
    {
      return;
    }

    if (state is not IReadOnlyList<KeyValuePair<string, object?>> formattedValues)
    {
      writer.WriteBoolean("useDefaultFormatter", true);
      writer.WriteString("message", formatter(state, null));
      return;
    }

    if (formattedValues.Count == 1 &&
      formattedValues[0].Key == ORIGINAL_FORMAT)
    {
      writer.WriteString("message", formatter(state, null));
      return;
    }

    foreach (var kvp in formattedValues)
    {
      if (kvp.Value is null || kvp.Key == ORIGINAL_FORMAT)
      {
        continue; // Skip null values and the OriginalFormat key
      }

      switch (kvp.Value)
      {
        case string strValue:
          writer.WriteString(kvp.Key, strValue);
          break;
        case DateTime dateTimeValue:
          writer.WriteString(kvp.Key, dateTimeValue);
          break;
        case DateTimeOffset dateTimeOffsetValue:
          writer.WriteString(kvp.Key, dateTimeOffsetValue);
          break;
        case Guid guidValue:
          writer.WriteString(kvp.Key, guidValue);
          break;
        case TimeSpan timeSpanValue:
          writer.WriteString(kvp.Key, timeSpanValue.ToString());
          break;
        case Enum enumValue:
          writer.WriteStartObject(kvp.Key);
          writer.WriteNumber("value", Convert.ToInt32(enumValue));
          writer.WriteString("name", enumValue.ToString());
          writer.WriteEndObject();
          break;
        case bool boolValue:
          writer.WriteBoolean(kvp.Key, boolValue);
          break;
        case ushort uShortValue:
          writer.WriteNumber(kvp.Key, uShortValue);
          break;
        case short shortValue:
          writer.WriteNumber(kvp.Key, shortValue);
          break;
        case uint uIntValue:
          writer.WriteNumber(kvp.Key, uIntValue);
          break;
        case int intValue:
          writer.WriteNumber(kvp.Key, intValue);
          break;
        case ulong uLongValue:
          writer.WriteNumber(kvp.Key, uLongValue);
          break;
        case long longValue:
          writer.WriteNumber(kvp.Key, longValue);
          break;
        case float floatValue:
          writer.WriteNumber(kvp.Key, floatValue);
          break;
        case double doubleValue:
          writer.WriteNumber(kvp.Key, doubleValue);
          break;
        case decimal decimalValue:
          writer.WriteNumber(kvp.Key, decimalValue);
          break;
        case sbyte sByteValue:
          writer.WriteNumber(kvp.Key, sByteValue);
          break;
        case byte byteValue:
          writer.WriteNumber(kvp.Key, byteValue);
          break;
        default:
          writer.WriteString(kvp.Key, kvp.Value.ToString());
          break;
      }
    }
  }
}
