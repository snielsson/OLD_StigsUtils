// Copyright © 2006-2024 Stig Schmidt Nielsson & Nielsson Consulting. All Rights Reserved.
// This file is Open Source and distributed under the MIT License - see LICENSE FILE.

using System.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StigsUtils.Exceptions;
namespace StigsUtils.Extensions;

public static class ServiceCollectionExtensions 
{
  // public static IServiceCollection AddInitializer(this IServiceCollection @this, Action<ServiceProvider> initializer) 
  // {
  //   @this.TryAddKeyedSingleton();
  // }
}

public static class ConfigurationExtensions
{
  public static string GetRequired(this IConfiguration configuration, string key, string? errorMessage = null)
    => configuration.GetNullable(key) ?? throw configuration.Error(errorMessage ?? $"Config key not found: {key}");
  public static string? GetNullable(this IConfiguration configuration, string? key) => key == null ? null : configuration[key];
}

public static class LinqExtensions 
{
  public static IEnumerable<T> WhichAre<T>(this IEnumerable @this) 
  {
    foreach (var x in @this)
    {
      if(x is T to) yield return to;
    }
  }
  public static void Dispose(this IEnumerable @this)
  {
    foreach (var x in @this)
    {
      if(x is IDisposable d) d.Dispose();
    }
  }
  public static void Invoke<T>(this IEnumerable<T> @this, Action<T> action)
  {
    foreach (var x in @this) action(x);
  }
} 