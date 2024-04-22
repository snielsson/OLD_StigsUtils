namespace StigsUtils;

public enum PathType
{
    Unspecified,
    DirPath,
    FilePath
}

internal class FileSystemPath : IPath 
{

    protected readonly string _value;
    public FileSystemPath(string value, PathType pathType = Pat
    )
    {
        _value = Path.GetFullPath(value);
    }
    public string Value => _value;
    public PathType PathType { get; }

}


internal class FilePath : FileSystemPath, IFilePath
{
    public FilePath(string value) : base(value)
    {
    }
}

internal class DirPath : FileSystemPath, IDirPath
{
    public DirPath(string value) : base(value)
    {
    }
}
}
