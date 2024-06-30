// Copyright © 2006-2024 Stig Schmidt Nielsson & Nielsson Consulting. All Rights Reserved.
// This file is Open Source and distributed under the MIT License - see LICENSE FILE.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace StigsUtils;

public class AmbientContext
{
  public static IServiceProvider? GlobalServiceProvider { get; set; }
  private static readonly AsyncLocal<AmbientContext> _current = new();
  public static AmbientContext Current
  {
    get => _current.Value ??= new AmbientContext();
    set => _current.Value = value;
  }
  private IServiceProvider? _serviceProvider;
  public static IServiceProvider ServiceProvider
  {
    get => Current._serviceProvider ?? GlobalServiceProvider ?? throw new InvalidOperationException("No ServiceProvider has been set in Ambient Context and AmbientContext.GlobalService provider has not been set.");
    set => Current._serviceProvider = value;
  }
}

public static class AmbientContextExtensions
{
  /// <summary>
  /// Convenience extension allowing any instance of type T to get an ILogger<T>.
  /// </summary>
  public static ILogger<T> Logger<T>(this T _)
    => AmbientContext.ServiceProvider
                     .GetRequiredService<ILoggerFactory>()
                     .CreateLogger<T>();
  public static ILogger Logger(this object x)
    => AmbientContext.ServiceProvider
                     .GetRequiredService<ILoggerFactory>()
                     .CreateLogger(x.GetType());
}