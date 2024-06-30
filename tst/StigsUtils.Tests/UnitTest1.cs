using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NUnit.Framework;
using StigsUtils.Applications;
using StigsUtils.Logging;
using ZLogger;

namespace StigsUtils.Tests;

public class CombinedTextWriters : TextWriter
{
  private readonly TextWriter? _original;
  private readonly Action<TextWriter>? _restore;
  private HashSet<TextWriter> _items = new();
  public CombinedTextWriters(TextWriter original, Action<TextWriter> restore, params TextWriter[] textWriters)
  {
    if (textWriters.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(textWriters));
    _original = original;
    _restore = restore;
    _items.Add(original);
    foreach (TextWriter x in textWriters) _items.Add(x);
  }
  public override void Write(char value)
  {
    foreach (TextWriter item in _items)
    {
      var b = item == TestContext.Progress;
      if(b) item.Write("!");
      item.Write(value);
      
    }
  }
  protected override void Dispose(bool disposing)
  {
    foreach (TextWriter item in _items)
    {
      if(item != _original) item.Dispose();      
    }
    if (_original != null && _restore != null) _restore(_original);
  }
  public override Encoding Encoding => _items.FirstOrDefault()?.Encoding ?? Encoding.UTF8;
}
public static class TextWriterExtensions 
{
  public static TextWriter CombineWith(this TextWriter @this, Action<TextWriter> restore, params TextWriter[] items)
  {
    if (items.Length == 0) return @this;
    return new CombinedTextWriters(@this, restore, items);
  }
}


public class Tests
{
  [SetUp]
  public void Setup()
  {
    Console.SetOut(Console.Out.CombineWith(x => Console.SetOut(x), TestContext.Progress));
  }
  [Test]
  public void AppBuilderWorks()
  {
    IServiceProvider app = App.Services
                              .SetMinimumLogLevel(LogLevel.Trace)
                              .AddConsoleLogger().Build();
    ILoggerFactory loggerFactory = app.GetRequiredService<ILoggerFactory>();
    ILogger logger = loggerFactory.CreateLogger("TEST");
    logger.LogInformation("1 dsadadsds");
    logger.ZLogInformation($"2 Hello my name is years old.");
    // int val = 23;
    Console.WriteLine("3 dddsd");
    // this.Logger().LogInformation($"1111 {val} fsfsdf");
    // this.Logger().LogInformation("2222 {SomeVal} fsfsdf",val);
  }
}