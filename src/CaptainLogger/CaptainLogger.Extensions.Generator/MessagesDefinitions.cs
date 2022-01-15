﻿
namespace CaptainLogger.Extensions.Generator;

internal static class MessagesDefinitions
{
    internal static string Get(int arguments, string[] templates)
    {
        var sb = new StringBuilder();
        sb
            .Append($"// <auto-generated last-generation=\"{DateTime.Now}\" arguments-count=\"{arguments}\" />")
            .Append(@$"
using System;
using Microsoft.Extensions.Logging;

namespace CaptainLogger;

#nullable enable");

        if (arguments > 0)
        {
            AddClasses(sb, arguments, templates);
        }

        sb.Append(@"
#nullable disable");

        return sb.ToString();
    }
    
    private static void AddClasses(
        StringBuilder sb,
        int arguments,
        string[] templates)
    {
        for (int i = 0; i < arguments; i++)
        {
            sb.Append(@$"
internal static class CptLoggerMessagesDefinitions{i + 1}<{GetGenericArgs(i)}>
{{
");

            for (int g = 1; g < 7; g++)
                AddMethods(sb, i, GetLogLevel(g), templates);

            sb.Append(@"}
");
        }
    }

    private static void AddMethods(
        StringBuilder sb,
        int arguments,
        string level,
        string[] templates)
    {

        sb.Append(@$"
    private static readonly Action<ILogger, {GetGenericArgs(arguments)}, Exception?> {level}Action{arguments + 1} = LoggerMessage
        .Define<{GetGenericArgs(arguments)}>(LogLevel.{level}, 0, {templates[arguments]});
");

        sb.Append($@"
    internal static void {level}Log(ILogger logger, {GetSignatureArgs(arguments)}) => {level}Action{arguments + 1}(logger, {GetCallParamArgs(arguments)},
                null);
");

        sb.Append($@"
    internal static void {level}Log(ILogger logger, {GetSignatureArgs(arguments)},
        Exception ex) => {level}Action{arguments + 1}(logger, {GetCallParamArgs(arguments)},
                ex);
");
    }
}