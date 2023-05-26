#nullable enable
using System;
using System.IO;

namespace Atheneum;

public class ScribeSettings
{
    private DirectoryInfo? _documentationDirectory;
    /// <summary>
    /// The Path to the documentation parent directory.
    /// </summary>
    public DirectoryInfo? DocumentationDirectory
    {
        get
        {
            return _documentationDirectory;
        }
        set
        {
            if (null == value) { return; }
            if (!value.Exists)
            {
                throw new FileNotFoundException("The provided directory could not be found");
            }
            _documentationDirectory = value;
        }
    }
    private FileInfo? _contributorsJsonPath;

    /// <summary>
    /// The Path to the JSON File where the Contributors information is stored.
    /// </summary>
    public FileInfo? ContributorsJsonPath
    {
        get { return _contributorsJsonPath; }
        set
        {
            if (null == value) { return; }
            if (System.IO.Path.GetExtension(value.Name) != ".json")
            {
                throw new NotSupportedException("Only files with the extension .JSON are allowed");
            }
            if (!value.Exists)
            {
                throw new FileNotFoundException("The provided file could not be found");
            }
            _contributorsJsonPath = value;
        }
    }

    public ScribeSettings()
    {
    }
}
