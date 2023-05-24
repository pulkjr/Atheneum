using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Atheneum;

/// <summary>
/// The base class that holds all of the knowledge
/// </summary>
public class Atheneum
{

    /// <summary>
    /// The Path to the documentation parent directory
    /// </summary>
    public DirectoryInfo Path;

    private FileInfo _contributorsJsonPath;
    /// <summary>
    /// The Path to the JSON File where the Contributors information is stored.
    /// </summary>
    public FileInfo ContributorsJsonPath
    {
        get { return _contributorsJsonPath; }
        set
        {
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

    /// <summary>
    /// The Articles held within the Atheneum
    /// </summary>
    public List<Article> Articles = new();

    /// <summary>
    /// A list of members in the Atheneum Team
    /// </summary>
    public List<Contributor> Contributors = new();

    /// <summary>
    /// Constructor for Atheneum Object
    /// </summary>
    /// <param name="path">The path to the Documentation Directory holding each of the technology directories</param>
    public Atheneum(DirectoryInfo path)
    {
        Path = path;
        InitializeItems();
    }

    /// <summary>
    /// An Empty constructor for using the Contributors methods only
    /// </summary>
    public Atheneum() { }

    /// <summary>
    /// Initialize the different document types within each technology directory
    /// </summary>
    /// <param name="includeFileContent"></param>
    public void InitializeItems()
    {
        // Create the YAML Deserializer
        IDeserializer yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
            .WithRequiredPropertyValidation()
            .WithTypeConverter(new YamlStringEnumConverter())
            .IgnoreUnmatchedProperties()
            .Build();

        string[] extensions = { ".md", ".yaml" };

        foreach (FileInfo _file in Path.EnumerateFiles("*", SearchOption.AllDirectories)
            .Where(f => extensions.Any(ext => ext == System.IO.Path.GetExtension(f.Name))))
        {

            string markdown = File.ReadAllText(_file.FullName);

            MarkdownDocument document = Markdown.Parse(markdown, Atheneum.Pipeline);

            // extract the first YAML frontmatter block
            // and assemble it into a string
            YamlFrontMatterBlock yamlBlock = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
            string yaml = yamlBlock?.Lines.ToString() ?? "";

            // Remove the Yaml FrontMatterBlock from the document
            _ = document.Remove(document.Descendants<YamlFrontMatterBlock>().FirstOrDefault());

            // create a YAML deserializer
            // and deserialize YAML into a dictionary (or object)
            TempBase _type = yamlDeserializer.Deserialize<TempBase>(yaml);

            string _normalizedType = _type.Type.Replace(" ", "").ToLower();

            Enums.ArticleType _articleType;

            if (Enum.TryParse(_normalizedType, true, out _articleType))
            {
                // Instantiate the article
                yaml = yamlBlock?.Lines.ToString() ?? "";
                Article _article = yamlDeserializer.Deserialize<Article>(yaml);

                // Add the Path to the Object
                _article.Path = _file;

                // Add the Markdown Object to the Object
                _article.Markdown = document;

                Articles.Add(_article);
            }
            else if (string.Compare(_normalizedType, "plan", StringComparison.CurrentCultureIgnoreCase) == 0)
            {

            }
            else if (string.Compare(_normalizedType, "test", StringComparison.CurrentCultureIgnoreCase) == 0)
            {

            }
            else if (string.Compare(_normalizedType, "a025", StringComparison.CurrentCultureIgnoreCase) == 0)
            {

            }
            else if (string.Compare(_normalizedType, "rtm", StringComparison.CurrentCultureIgnoreCase) == 0)
            {

            }
            else if (string.Compare(_normalizedType, "procedure", StringComparison.CurrentCultureIgnoreCase) == 0)
            {

            }
            else if (string.Compare(_normalizedType, "stig", StringComparison.CurrentCultureIgnoreCase) == 0)
            {

            }
            else if (string.Compare(_normalizedType, "rule", StringComparison.CurrentCultureIgnoreCase) == 0)
            {

            }
            else
            {
                throw new InvalidDataException($"The provided type [{_normalizedType}]is unhandled");
            }
        }
    }

    /// <summary>
    /// Import the information about the contributors from a json file
    /// </summary>
    /// <param name="jsonFilePath">The path to the JSON file</param>
    public void ImportContributors(FileInfo jsonFilePath)
    {
        ContributorsJsonPath = jsonFilePath;
        string jsonString = File.ReadAllText(ContributorsJsonPath.FullName);
        Contributors = JsonSerializer.Deserialize<List<Contributor>>(jsonString);
    }
    /// <summary>
    /// Save the current Contributors status in memory to disk
    /// </summary>
    /// <returns>The location of the current JSON File</returns>
    public void SaveContributors()
    {
        _ = ExportContributors(ContributorsJsonPath);
    }
    /// <summary>
    /// Export the current Contributors status in memory to a new location on disk
    /// </summary>
    /// <param name="Path">The path where you want to place the file</param>
    public async Task ExportContributors(FileInfo Path)
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true,
            MaxDepth = 3
            //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        using var stream = File.Create(Path.FullName);
        await JsonSerializer.SerializeAsync(stream, Contributors);
        await stream.FlushAsync();
        stream.Close();
    }
    /// <summary>
    /// Create a markdown parsing pipeline
    /// </summary>
    public static MarkdownPipeline Pipeline =>
            new MarkdownPipelineBuilder()
                .UseAutoLinks()
                .UseAutoIdentifiers()
                .UseYamlFrontMatter()
                .Build();
}
public class TempBase
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "The property 'type' is required")]
    [YamlMember(Alias = "type")]
    public string Type;
}