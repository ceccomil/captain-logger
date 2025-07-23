global using CaptainLogger.Extensions.Generator.Generators;
global using CaptainLogger.Extensions.Generator.Tests.Helpers;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.Extensions.DependencyModel;
global using System.Collections.Immutable;
global using System.Reflection;
global using Xunit;
global using GenResults = (
  System.Collections.Generic.List<CaptainLogger.Extensions.Generator.Tests.Helpers.GeneratedFile> Generated,
  Microsoft.CodeAnalysis.Compilation? OutCompilation,
  System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);
