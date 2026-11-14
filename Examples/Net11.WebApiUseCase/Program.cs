global using CaptainLogger;
global using Microsoft.AspNetCore.Mvc;
global using System.Collections.Immutable;
using WebApiUseCase;

var builder = WebApplication
  .CreateBuilder(args);

builder.AddRegistrations();

var app = builder.Build();

app.UseRegistrations();

await app.RunAsync();
