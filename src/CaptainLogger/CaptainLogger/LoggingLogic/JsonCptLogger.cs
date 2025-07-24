namespace CaptainLogger.LoggingLogic;

internal class JsonCptLogger(
  string category,
  string provider,
  Func<CaptainLoggerOptions> getCurrentConfig,
  Func<LoggerFilterOptions> getCurrentFilters,
  Func<CaptainLoggerEventArgs<object>, Task> onLogEntry)
  : CptLoggerBase(
    category,
    provider,
    getCurrentConfig,
    getCurrentFilters,
    onLogEntry)
{
  private const string ORIGINAL_FORMAT = "{OriginalFormat}";
  private const string MESSAGE = "message";

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
      config,
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
    CaptainLoggerOptions config,
    Func<TState, Exception?, string> formatter)
  {
    var buffer = new ArrayBufferWriter<byte>();
    using var writer = new Utf8JsonWriter(buffer);

    writer.WriteStartObject();
    writer.WriteString("timestamp", time);
    writer.WriteString("severity", level.ToCaptainLoggerString());

    writer.WriteStartObject("event");
    writer.WriteNumber("id", eventId.Id);

    if (!string.IsNullOrWhiteSpace(eventId.Name))
    {
      writer.WriteString("name", eventId.Name);
    }

    writer.WriteEndObject();

    if (CaptainLoggerCorrelationScope.TryGetCorrelationId(
      out var correlationIdValue))
    {
      writer.WriteString("correlationId", correlationIdValue);
    }

    writer.WriteString("source", Category);

    WriteContent(writer, state, formatter);

    if (!config.DoNotAppendException && ex is not null)
    {
      writer.WriteStartObject("exception");
      writer.WriteString(MESSAGE, ex.Message);
      writer.WriteString("stackTrace", ex.StackTrace);
      writer.WriteEndObject();
    }

    WriteAdditionalProperties(config, writer);

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
      writer.WriteString(MESSAGE, formatter(state, null));

      return;
    }

    var atleastOneMessage = false;

    foreach (var kvp in formattedValues)
    {
      if (kvp.Value is null || kvp.Key == ORIGINAL_FORMAT)
      {
        continue; // Skip null values and the OriginalFormat key
      }

      writer.WritePropertyName(kvp.Key);

      if (kvp.Key == MESSAGE)
      {
        atleastOneMessage = true;
      }

      WritePrimitive(writer, kvp.Value);
    }

    if (!atleastOneMessage)
    {
      writer.WriteString(MESSAGE, formatter(state, null));
    }
  }

  private static void WriteAdditionalProperties(
    CaptainLoggerOptions config,
    Utf8JsonWriter writer)
  {
    foreach (var kvp in config.StructuredLogMetadata)
    {
      if (kvp.Value is null)
      {
        continue; // Skip null values
      }

      writer.WritePropertyName(kvp.Key);
      WritePrimitive(writer, kvp.Value);
    }
  }

  private static void WritePrimitive(
    Utf8JsonWriter writer,
    object value)
  {
    if (value is byte[] byteArray)
    {
      writer.WriteStringValue(Convert.ToBase64String(byteArray));
      return;
    }

    if (value is not string &&
      value is IEnumerable enumerable)
    {
      writer.WriteStartArray();

      foreach (var item in enumerable)
      {
        if (item is null)
        {
          writer.WriteNullValue();
          continue;
        }

        WritePrimitive(writer, item);
      }

      writer.WriteEndArray();

      return;
    }

    switch (value)
    {
      case string strValue:
        writer.WriteStringValue(strValue);
        break;
      case DateTime dateTimeValue:
        writer.WriteStringValue(dateTimeValue);
        break;
      case DateTimeOffset dateTimeOffsetValue:
        writer.WriteStringValue(dateTimeOffsetValue);
        break;
      case Guid guidValue:
        writer.WriteStringValue(guidValue);
        break;
      case TimeSpan timeSpanValue:
        writer.WriteStringValue(timeSpanValue.ToString());
        break;
      case Enum enumValue:
        writer.WriteStartObject();
        writer.WriteNumber("value", Convert.ToInt32(enumValue));
        writer.WriteString("name", enumValue.ToString());
        writer.WriteEndObject();
        break;
      case bool boolValue:
        writer.WriteBooleanValue(boolValue);
        break;
      case ushort uShortValue:
        writer.WriteNumberValue(uShortValue);
        break;
      case short shortValue:
        writer.WriteNumberValue(shortValue);
        break;
      case uint uIntValue:
        writer.WriteNumberValue(uIntValue);
        break;
      case int intValue:
        writer.WriteNumberValue(intValue);
        break;
      case ulong uLongValue:
        writer.WriteNumberValue(uLongValue);
        break;
      case long longValue:
        writer.WriteNumberValue(longValue);
        break;
      case float floatValue:
        writer.WriteNumberValue(floatValue);
        break;
      case double doubleValue:
        writer.WriteNumberValue(doubleValue);
        break;
      case decimal decimalValue:
        writer.WriteNumberValue(decimalValue);
        break;
      case sbyte sByteValue:
        writer.WriteNumberValue(sByteValue);
        break;
      case byte byteValue:
        writer.WriteNumberValue(byteValue);
        break;
      default:
        writer.WriteStringValue(value.ToString());
        break;
    }
  }
}
