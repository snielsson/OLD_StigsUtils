// Copyright © 2006-2024 Stig Schmidt Nielsson & Nielsson Consulting. All Rights Reserved.
// This file is Open Source and distributed under the MIT License - see LICENSE FILE.

using StigsUtils.Extensions;
using ZLogger;

namespace StigsUtils.Logging;

public interface ILogMessageHandlerProcessor 
{ 
}

public class LogMessageHandlerProcessor<T> : ILogMessageHandlerProcessor, IAsyncLogProcessor, IDisposable
{
  private readonly HashSet<ILogMessageHandler<T>> _handlers = new();
  public ValueTask DisposeAsync()
  {
    Dispose();
    return ValueTask.CompletedTask;
  }
  public void Post(IZLoggerEntry log)
  {
    // ReSharper disable once InconsistentlySynchronizedField
    _handlers.Invoke(x => x.Handle(new LogMessage<T>
    (
      log.LogInfo.LogLevel,
      log.ToString(),
      log.LogInfo.Category.Name,
      log.LogInfo.EventId,
      log.LogInfo.Exception,
      default
    )));
  }
  public void Dispose()
  {
    _handlers.Dispose();
  }
  public LogMessageHandlerProcessor<T> Add(ILogMessageHandler<T> handler)
  {
    lock (_handlers)
    {
      if (_handlers.Contains(handler)) throw new InvalidOperationException("Handler has already been added: " + handler);
      if (!_handlers.Add(handler)) throw new InvalidOperationException("Failed to add handler");
    }
    return this;
  }
}