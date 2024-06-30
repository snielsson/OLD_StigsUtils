// // Copyright © 2006-2024 Stig Schmidt Nielsson & Nielsson Consulting. All Rights Reserved.
// // This file is Open Source and distributed under the MIT License - see LICENSE FILE.
//
// using System.Threading.Tasks.Dataflow;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.DependencyInjection.Extensions;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
//
// namespace StigsUtils.Applications;
//
// public static class ActionLoggerExtensions
// {
//   public static IServiceCollection AddActionLogger(this IServiceCollection @this, Action<IServiceProvider, ActionLogger> configure)
//     => @this.AddSingleton<ILoggerProvider, ActionLogger>(serviceProvider =>
//     {
//       var result = new ActionLogger();
//       configure(serviceProvider, result);
//       return result;
//     });
// }
// public class ActionLogger : ILogger, ILoggerProvider
// {
//   private readonly string _category;
//   private readonly List<(Func<LogMessage, bool>? predicate, Func<LogMessage, Task>)> _handlers = [];
//
//   private readonly ActionBlock<Func<Task>> _queue = new(async x =>
//     {
//       try
//       {
//         await x.Invoke();
//       }
//       catch (Exception) { }
//     }
//   );
//   public ActionLogger() { }
//   public ActionLogger(string category)
//   {
//     _category = category;
//   }
//   public LogLevel MaxLevel { get; set; }
//   public LogLevel MinLevel { get; set; }
//
//   public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
//   {
//     LogMessage? message = null;
//     foreach (var (predicate, action) in _handlers)
//     {
//       if (predicate != null && predicate(message ??= new LogMessage(logLevel, _category, eventId, exception, formatter(state, exception)))) continue;
//       var x = message ??= new LogMessage(logLevel, _category, eventId, exception, formatter(state, exception));
//       _queue.Post(() => action(x));
//     }
//   }
//   public bool IsEnabled(LogLevel logLevel) => logLevel >= MinLevel && logLevel <= MaxLevel;
//   public IDisposable? BeginScope<TState>(TState state) where TState : notnull => throw new NotImplementedException();
//   public void Dispose()
//   {
//     _queue.Complete();
//     _queue.Completion.Wait();
//   }
//   public ILogger CreateLogger(string categoryName) => new ActionLogger(categoryName);
//   public record LogMessage(
//     LogLevel LogLevel,
//     string Category,
//     EventId EventId,
//     Exception? Exception,
//     string Message);
// }
// public class ConsoleApplication : IConsoleApplication
// {
//   private readonly HostApplicationBuilder _builder;
//   private readonly Lazy<IHost> _host;
//   public ConsoleApplication()
//   {
//     _builder = new HostApplicationBuilder();
//     _builder.Logging.AddProvider()
//     _host = new Lazy<IHost>(() => _builder.Build());
//   }
//   public TextWriter StdOut { get; set; } = Console.Out;
//   public TextWriter StdErr { get; set; } = Console.Error;
//   public IServiceCollection Services => _builder.Services;
//   public IServiceProvider ServiceProvider => Host.Services;
//   public IHost Host => _host.Value;
//   public async Task<int> Run(Func<string[], Task<int>> main, string[]? args = null)
//   {
//     try
//     {
//       return await main(args ?? Environment.GetCommandLineArgs());
//     }
//     catch (Exception ex)
//     {
//       StdErr.WriteLine(ex);
//       return -1;
//     }
//   }
// }