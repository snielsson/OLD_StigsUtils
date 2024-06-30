// Copyright © 2024 TradingLens. All Rights Reserved.

using FluentAssertions;

namespace Scripts;

public class DirPath
{
  public DirPath(string path) : this(new DirectoryInfo(path)) { }
  public DirPath(DirectoryInfo info)
  {
    Info = info;
  }
  public DirPath ShouldExist(string? because = null)
  {
    Info.Exists.Should().BeTrue(because);
    return this;
  }
  private DirectoryInfo Info { get; }
  public string Name => Info.Name;
  public DirectoryInfo DirectoryInfo => Info;
  public static implicit operator DirPath(DirectoryInfo info) => new(info);
  public static implicit operator DirPath(string path) => new(path);

  public DirPath Combine<T>(T? t1)
  {
    var combined = Path.Combine(Info.ToString(), t1?.ToString() ?? "");
    return new DirPath(combined);
  }
  public DirPath Combine<T1, T2>(T1? t1, T2? t2)
  {
    var combined = Path.Combine(Info.ToString(), t1?.ToString() ?? "", t2?.ToString() ?? "");
    return new DirPath(combined);
  }
  public DirPath Combine<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
  {
    var combined = Path.Combine(Info.ToString(), t1?.ToString() ?? "", t2?.ToString() ?? "", t3?.ToString() ?? "");
    return new DirPath(combined);
  }
  public DirPath Combine(object? arg)
  {
    var combined = Path.Combine(Info.ToString(), arg?.ToString() ?? "");
    return new DirPath(combined);
  }
  public DirPath Combine(params object[] args)
  {
    var combined = Path.Combine(args.Select(x => x.ToString() ?? "").Prepend(Info.ToString()).ToArray());
    return new DirPath(combined);
  }
  public static implicit operator string(DirPath x) => x.ToString() ?? "";
  // public static implicit operator DirPath(string x) => new(x);
  public static DirPath operator /(DirPath lhs, object rhs) => lhs.Combine(rhs);
  public static DirPath CreateDirectory(string path) => Directory.CreateDirectory(path);
  private DirPath Create()
  {
    Info.Create();
    return this;
  }
  private DirPath CreateSubdirectory(string path) => Info.CreateSubdirectory(path);
  /// <summary>
  ///   Get file paths for all files in directory and subdirectories.
  /// </summary>
  public IEnumerable<FilePath> AllFiles(string searchPattern = "*") => Files(searchPattern, SearchOption.AllDirectories);
  /// <summary>
  ///   Get file paths for all files in directory and optionally subdirectories.
  /// </summary>
  public IEnumerable<FilePath> Files(string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
    => DirectoryInfo.EnumerateFiles(searchPattern, searchOption).Select(x => new FilePath(x.ToString()));

  public DirPath CopyTo(DirPath destination, bool overwrite = false, bool onlyChanged = true)
  {
    Info.CopyTo(destination.Info,overwrite, onlyChanged);
    return this;
  }
  public DirPath MirrorTo(DirPath destination, bool onlyChanged = true)
  {
    Info.MirrorTo(destination.Info, onlyChanged);
    return this;
  }
}