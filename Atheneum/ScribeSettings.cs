#nullable enable
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Atheneum;

public class ScribeSettings
{
    [JsonIgnore]
    private DirectoryInfo _documentationDirectory;
    /// <summary>
    /// The Path to the documentation parent directory.
    /// </summary>
    [JsonConverter(typeof(DirectoryInfoConverter))]
    public DirectoryInfo DocumentationDirectory
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
                throw new DirectoryNotFoundException("The documentation directory could not be found");
            }
            _documentationDirectory = value;
        }
    }
    [JsonIgnore]
    private FileInfo? _contributorsJsonPath;

    /// <summary>
    /// The Path to the JSON File where the Contributors information is stored.
    /// </summary>
    [JsonConverter(typeof(FileInfoConverter))]
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
                value.Create();
            }
            _contributorsJsonPath = value;
        }
    }
    [JsonIgnore]
    private DirectoryInfo _markdownTemplateDirectory;
    /// <summary>
    /// The Path to the documentation parent directory.
    /// </summary>
    [JsonConverter(typeof(DirectoryInfoConverter))]
    public DirectoryInfo MarkdownTemplateDirectory
    {
        get
        {
            return _markdownTemplateDirectory;
        }
        set
        {
            if (null == value) { return; }
            if (!value.Exists)
            {
                throw new DirectoryNotFoundException("The markdown template directory could not be found");
            }
            _markdownTemplateDirectory = value;
        }
    }
    [JsonIgnore]
    private FileInfo _settingsJsonPath;

    /// <summary>
    /// The Path to the JSON File where the Contributors information is stored.
    /// </summary>
    [JsonIgnore]
    public FileInfo SettingsJsonPath
    {
        get { return _settingsJsonPath; }
        set
        {
            if (null == value) { return; }
            if (System.IO.Path.GetExtension(value.Name) != ".json")
            {
                throw new NotSupportedException("Only files with the extension .JSON are allowed");
            }

            _settingsJsonPath = value;
        }
    }

    public ScribeSettings()
    {
    }
    public ScribeSettings(FileInfo settingsJsonPath, DirectoryInfo documentationPath, FileInfo? contributorJsonPath)
    {
        SettingsJsonPath = settingsJsonPath;
        DocumentationDirectory = documentationPath;

        if (contributorJsonPath != null)
        {
            ContributorsJsonPath = contributorJsonPath;
        }
    }
    public void Save()
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true,
            MaxDepth = 3,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        string jsonString = JsonSerializer.Serialize<ScribeSettings>(this, options);

        File.WriteAllText(SettingsJsonPath.FullName, jsonString);
    }
}
