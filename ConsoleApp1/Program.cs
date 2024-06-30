// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Logging;
using StigsUtils;
using StigsUtils.Applications;
using StigsUtils.Logging;

IServiceProvider app = App.Services
                          .SetMinimumLogLevel(LogLevel.Trace)
                          .AddConsoleLogger()
                          .Build();
 
Console.WriteLine("Hello, World!");

app.Logger().LogInformation("dsdsdsds");
