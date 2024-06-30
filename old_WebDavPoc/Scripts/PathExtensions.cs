// Copyright © 2024 TradingLens. All Rights Reserved.

namespace Scripts;

public static class PathExtensions
{
  public static DirPath AsExistingDir(this string @this,string? because = null) => new DirPath(@this).ShouldExist(because);
  public static string Combine(this string @this, string x) => Path.Combine(@this, x);
  public static string Combine(this string @this, string x1, string x2) => Path.Combine(@this, x1, x2);
  public static string Combine(this string @this, string x1, string x2, string x3) => Path.Combine(@this, x1, x2, x3);
  public static string Combine(this string @this, params string[] args) => Path.Combine(args.Prepend(@this).ToArray());
  public static bool Includes<T>(this Predicate<T>? @this, T x, Predicate<T>? except = null)
  {
    if (@this != null && !@this(x)) return false;
    if (except != null && except(x)) return false;
    return true;
  }

  public static bool In<T>(this T? @this, T x1) => @this != null && @this.Equals(x1);
  public static bool In<T>(this T? @this, T x1, T x2) => @this != null && (@this.Equals(x1) || @this.Equals(x2) );
  public static bool In<T>(this T? @this, T x1, T x2, T x3) => @this != null && (@this.Equals(x1) || @this.Equals(x2) || @this.Equals(x3));
  public static bool In<T>(this T? @this, T x1, T x2, T x3, T x4) => @this != null && (@this.Equals(x1) || @this.Equals(x2) || @this.Equals(x3) || @this.Equals(x4));
  public static bool In<T>(this T? @this, params T[] args)
  {
    if (@this == null) return false;
    for (int i = 0; i < args.Length; i++)
    {
      if (@this.Equals(args[i])) return true;
    }
    return false;
  }
  // Copies all files and directories from source to destination, including empty directories
  public static void CopyTo(this DirectoryInfo sourceDir, DirectoryInfo targetDir, bool overwrite = true, bool onlyChanged = true)
  {
    targetDir.Create();
    int sourcePathLength = sourceDir.FullName.Length;
    if (sourceDir.FullName[^1].In('/', '\\')) sourcePathLength++;
    foreach (DirectoryInfo dir in sourceDir.GetDirectories("*", SearchOption.AllDirectories))
    {
      // Generate the relative directory path and concatenate it with the target directory path
      string relativePath = dir.FullName.Substring(sourcePathLength);
      string targetDirPath = Path.Combine(targetDir.FullName, relativePath);
      Directory.CreateDirectory(targetDirPath);
    }
    foreach (FileInfo file in sourceDir.GetFiles("*", SearchOption.AllDirectories))
    {
      string relativePath = file.FullName.Substring(sourcePathLength);
      string targetFilePath = Path.Combine(targetDir.FullName, relativePath);
      var targetInfo = new FileInfo(targetFilePath);
      if (targetInfo.Exists)
      {
        if (overwrite == false) continue;
        if (onlyChanged && targetInfo.Length == file.Length && targetInfo.LastWriteTimeUtc == file.LastWriteTimeUtc) continue;
      }
      file.CopyTo(targetFilePath);
    }
  }

  public static void MirrorTo(this DirectoryInfo sourceDir, DirectoryInfo targetDir, bool onlyChanged = true)
  {
    var targetDirExisted = targetDir.Exists;
    sourceDir.CopyTo(targetDir, true, onlyChanged);
    if (!targetDirExisted) return;
    int targetPathLength = targetDir.FullName.Length;
    if (targetDir.FullName.Last().In('/', '\\')) targetPathLength++;
    var targetDirs = targetDir.EnumerateDirectories().ToArray();
    foreach (var trgDir in targetDirs)
    {
      if (trgDir.Exists)
      {
        var srcDir = Path.Combine(sourceDir.FullName, trgDir.FullName.Substring(targetPathLength));
        if(!Directory.Exists(srcDir)) trgDir.Delete(true);
      }
    }
  }
}