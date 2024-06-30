// Copyright © 2006-2024 Stig Schmidt Nielsson & Nielsson Consulting. All Rights Reserved.
// This file is Open Source and distributed under the MIT License - see LICENSE FILE.

using System.Collections;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StigsUtils.Extensions;

namespace StigsUtils.Applications;

public interface IAppBuilder: IServiceCollection, ILoggingBuilder
{
  IConfiguration Configuration { get; }
  IServiceProvider Build(ServiceProviderOptions? options = null);
}

public class AppBuilder :  IAppBuilder
{
  public AppBuilder()
  {
    this.AddLogging().RemoveAll<ILoggerProvider>();
  }
  
  private static readonly Lazy<IConfigurationRoot> _nonEnvironmentSpecificConfiguration = new(LoadNonEnvironmentSpecificConfiguration);
  // private static Lazy<IHostEnvironment> _hostEnvironment = new(() => new HostingEnvironment
  // {
  //   EnvironmentName = Con.GetRequired(DefaultEnvironmentNameVariable),
  //   ApplicationName = Assembly.GetEntryAssembly()?.FullName ?? "",
  //   ContentRootPath = Assembly.GetEntryAssembly()?.Location ?? Environment.CurrentDirectory,
  //   ContentRootFileProvider = null!
  // });
  private IConfiguration? _configuration;
  private string? _environmentName;
  private string GetEnvironmentName(string? key = null) => Configuration.GetRequired(key ??DefaultEnvironmentNameVariable);

  public static IConfigurationRoot LoadNonEnvironmentSpecificConfiguration()
    => new ConfigurationManager()
      .AddJsonFile("appsettings.json")
      .AddUserSecrets(Assembly.GetEntryAssembly()!, true)
      .AddEnvironmentVariables()
      .AddCommandLine(Environment.GetCommandLineArgs())
      .Build();

  public static string DefaultEnvironmentNameVariable { get; set; } = "ASPNETCORE_ENVIRONMENT";
  public string EnvironmentName
  {
    get => _environmentName ??= GetEnvironmentName();
    set { _environmentName = value; }
  }
  
  public IAppBuilder ForEnvironments(string[] environmentNames, Action<IAppBuilder> action)
  {
    if (environmentNames.Contains(EnvironmentName, StringComparer.OrdinalIgnoreCase))
    {
      action(this);
    }
    return this;
  }
  
  public IAppBuilder ExceptEnvironments(string[] environmentNames, Action<IAppBuilder> action)
  {
    if (!environmentNames.Contains(EnvironmentName, StringComparer.OrdinalIgnoreCase))
    {
      action(this);
    }
    return this;
  }
  public IAppBuilder Production(Action<IAppBuilder> action) => ForEnvironments([Environments.Production], action);
  public IAppBuilder NonProduction(Action<IAppBuilder> action) => ExceptEnvironments([Environments.Production], action);
  
  public IServiceCollection Services { get; set; } = new ServiceCollection();
  public IEnumerator<ServiceDescriptor> GetEnumerator() => Services.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Services).GetEnumerator();
  public void Add(ServiceDescriptor item) => Services.Add(item);
  public void Clear() => Services.Clear();
  public bool Contains(ServiceDescriptor item) => Services.Contains(item);
  public void CopyTo(ServiceDescriptor[] array, int arrayIndex) => Services.CopyTo(array, arrayIndex);
  public bool Remove(ServiceDescriptor item) => Services.Remove(item);
  public int Count => Services.Count;
  public bool IsReadOnly => Services.IsReadOnly;
  public int IndexOf(ServiceDescriptor item) => Services.IndexOf(item);
  public void Insert(int index, ServiceDescriptor item) => Services.Insert(index, item);
  public void RemoveAt(int index) => Services.RemoveAt(index);
  public ServiceDescriptor this[int index]
  {
    get => Services[index];
    set => Services[index] = value;
  }
  public IConfiguration Configuration
  {
    get => _configuration ?? _nonEnvironmentSpecificConfiguration.Value;
    protected set => _configuration = value;
  }
  public IServiceProvider Build(ServiceProviderOptions? options = null) => AmbientContext.GlobalServiceProvider = Services.BuildServiceProvider(options ?? new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
}