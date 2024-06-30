// Copyright © 2024 TradingLens. All Rights Reserved.

namespace Scripts;

public class FilePath //: Path<FilePath>
{
  private FileInfo _fileInfo;
  public FilePath(string path) : this(new FileInfo(path)) { }
  public FilePath(FileInfo fileInfo)
  {
    _fileInfo = fileInfo;
  }
  public string FullName => _fileInfo.FullName;
  public static implicit operator FilePath(string x) => new(x);
  public static implicit operator string(FilePath x) => x.ToString()??"";
}