global using CaptainLogger.Contracts.Options;
global using CaptainLogger.Extensions.Generator.Definitions;
global using CaptainLogger.Extensions.Generator.Helpers;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using System.Collections.Immutable;
global using static CaptainLogger.Extensions.Generator.Helpers.Constants;
global using BaseProvider =
(
  System.Collections.Immutable.ImmutableArray<
    CaptainLogger.Extensions.Generator.Definitions.LoggerOptionsDefinition> Options,
  bool IsNullable
);
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CaptainLogger.Extensions.Generator.Tests")]