// Copyright © 2006-2024 Stig Schmidt Nielsson & Nielsson Consulting. All Rights Reserved.
// This file is Open Source and distributed under the MIT License - see LICENSE FILE.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StigsUtils.Applications;
using StigsUtils.Extensions;
using ZLogger;
using ZLogger.Providers;

namespace StigsUtils.Logging;

public interface ILogMessageHandler<T>
{
  public void Handle(LogMessage<T> logMessage);
}

public interface ILogMessageHandler
{
  public void Handle(LogMessage logMessage);
}

public static class LoggingExtensions
{
  public static IAppBuilder SetMinimumLogLevel(this IAppBuilder @this, LogLevel logLevel)
  {
    @this.SetMinimumLevel(logLevel);
    return @this;
  }

  public static IAppBuilder AddConsoleLogger(this IAppBuilder @this, Action<ZLoggerConsoleOptions,IServiceProvider>? configure = null)
  {
    @this.AddLogging();
    @this.AddZLoggerConsole((options, serviceProvider) =>
    {
      configure?.Invoke(options,serviceProvider);
    });
    return @this;
  }

  public static IAppBuilder AddLogMessageHandler<THandler,TContext>(this IAppBuilder @this) where THandler : class, ILogMessageHandler<TContext>
  {
    @this.AddLogging();
    @this.AddSingleton<LogMessageHandlerProcessor<TContext>>();
    @this.AddZLoggerLogProcessor((_, serviceProvider) =>
      {
        THandler handler = serviceProvider.GetRequiredService<THandler>();
        var processor = serviceProvider.GetRequiredService<LogMessageHandlerProcessor<TContext>>();
        return processor.Add(handler);
      });
    return @this;
  }

  public static IAppBuilder AddRollingFileLogger(this IAppBuilder @this, string? logDirectory = null, string? fileName = null)
  {
    logDirectory ??= @this.Configuration.GetNullable("LogDirectory") ??
                     Path.Combine(Assembly.GetEntryAssembly()?.Location ??
                                  Environment.CurrentDirectory,"Logs");
    Directory.CreateDirectory(logDirectory);
    fileName ??= Assembly.GetEntryAssembly()?.FullName??"";
    @this.AddLogging();
    @this.AddZLoggerRollingFile((options)=>
    {
      options.FilePathSelector = (timestamp, seq) =>
      {
        var seqString = seq == 0 ? "" : $".{seq:000}.";
        return Path.Combine(logDirectory, $"{fileName}.{timestamp.UtcDateTime:yyyyMMddK}.{seqString}.log");
      };
      options.RollingInterval = RollingInterval.Day;
      options.RollingSizeKB = 102400; //100mb
    });
    return @this;
  }
}

