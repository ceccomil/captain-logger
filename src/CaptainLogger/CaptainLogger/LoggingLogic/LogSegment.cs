﻿namespace CaptainLogger.LoggingLogic;

internal readonly struct LogSegment(
  string value,
  ConsoleColor color)
{
  public string Value { get; } = value;
  public ConsoleColor Color { get; } = color;

  public override string ToString() => Value;
}
