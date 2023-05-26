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
/// The base class that holds all of the knowledge documentation
/// </summary>
public class Scribe
{
    /// <summary>
    /// The control settings for this <see cref="Scribe"/> within Atheneum
    /// </summary>
    public ScribeSettings ScribeSettings;

    /// <summary>
    /// The list of <see cref="Article"/> held within the Atheneum
    /// </summary>
    public List<Article> Articles = new();

    /// <summary>
    /// A list of <see cref="Contributor"/> in the Atheneum
    /// </summary>
    public List<Contributor> Contributors = new();

    /// <summary>
    /// Constructor for a new <see cref="Scribe"/> within Atheneum. This will populate the <see cref="Scribe"/> using the settings provided.
    /// </summary>
    /// <param name="scribeSettings">The settings for this <see cref="Scribe"/></param>
    public Scribe(ScribeSettings scribeSettings)
    {
        ScribeSettings = scribeSettings;
        if (null != scribeSettings.DocumentationDirectory)
        {
            InitializeItems();
        }
        if (null != scribeSettings.ContributorsJsonPath)
        {
            ImportContributors();
        }
    }

    /// <summary>
    /// Constructor for an empty <see cref="Scribe"/> within Atheneum
    /// </summary>
    public Scribe() { }

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

        if (ScribeSettings.DocumentationDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories).Count() < 1)
        {
            throw new ArgumentException("No files were found in the documentation directory");
        }
        if (ScribeSettings.DocumentationDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories)
            .Where(f => extensions.Any(ext => ext == System.IO.Path.GetExtension(f.Name))).Count() < 1)
        {
            throw new ArgumentException("No files were found in the documentation directory with extensions .md or .yaml");
        }

        foreach (FileInfo _file in ScribeSettings.DocumentationDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories)
            .Where(f => extensions.Any(ext => ext == Path.GetExtension(f.Name))))
        {
            string fileContent = File.ReadAllText(_file.FullName);

            if (string.IsNullOrEmpty(fileContent))
            {
                continue;
            }
            if (Path.GetExtension(_file.Name) == ".yaml")
            {
                continue;
            }
            MarkdownDocument document;
            try
            {
                document = Markdown.Parse(fileContent, Scribe.Pipeline);
            }
            catch (Exception e)
            {
                throw new InvalidDataException($"There was a problem loading the markdown content for {_file.FullName}. The error received was {e.GetBaseException().Message}");
            }

            // extract the first YAML frontmatter block
            // and assemble it into a string
            YamlFrontMatterBlock yamlBlock;
            try
            {
                yamlBlock = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
            }
            catch (Exception e)
            {
                continue;
            }
            if (null == yamlBlock || yamlBlock?.Lines.Count < 1)
            {
                continue;
            }
            string yaml = yamlBlock?.Lines.ToString() ?? "";

            if (string.IsNullOrEmpty(yaml))
            {
                continue;
            }
            // Remove the Yaml FrontMatterBlock from the document
            try
            {
                _ = document.Remove(document.Descendants<YamlFrontMatterBlock>().FirstOrDefault());
            }
            catch (Exception e)
            {
                throw new InvalidCastException($"There was an issue during the removal of the frontmatter: Error was {e.GetBaseException().Message}");
            }

            TempBase _tempBase;
            try
            {
                // Deserialize to TempBase to figure out what class to instantiate.
                _tempBase = yamlDeserializer.Deserialize<TempBase>(yaml);
            }
            catch (Exception e)
            {
                throw new InvalidCastException($"There was an issue during the deserialization of {_file.FullName}. Error was {e.GetBaseException().Message}");
            }
            if (null == _tempBase)
            {
                throw new InvalidCastException($"There was an issue during the deserialization of {_file.FullName}. Returned class is null.");
            }
            if (string.IsNullOrEmpty(_tempBase.DocumentType))
            {
                throw new InvalidCastException($"File [{_file.FullName}] property type is null or empty.");
            }
            string _normalizedDocumentType = _tempBase.DocumentType.Replace(" ", "").ToLower();

            Enums.ArticleType _articleType;

            if (Enum.TryParse<Enums.ArticleType>(_normalizedDocumentType, true, out _articleType))
            {
                // Instantiate the article
                yaml = yamlBlock?.Lines.ToString() ?? "";

                Article _article;
                try
                {
                    _article = yamlDeserializer.Deserialize<Article>(yaml);
                }
                catch (Exception e)
                {
                    throw new InvalidCastException($"There was an issue during the deserialization of {_file.FullName} to an Article. Error was {e.GetBaseException().Message}");
                }

                // Add the Path to the Object
                _article.Path = _file;

                // Add the Markdown Object to the Object
                _article.Markdown = document;

                Articles.Add(_article);
            }
            else if (string.Compare(_normalizedDocumentType, "plan", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                continue;
            }
            else if (string.Compare(_normalizedDocumentType, "test", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                continue;
            }
            else if (string.Compare(_normalizedDocumentType, "a025", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                continue;
            }
            else if (string.Compare(_normalizedDocumentType, "rtm", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                continue;
            }
            else if (string.Compare(_normalizedDocumentType, "install", StringComparison.CurrentCultureIgnoreCase) == 0 || string.Compare(_normalizedDocumentType, "reference", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                continue;
            }
            else if (string.Compare(_normalizedDocumentType, "procedure", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                continue;
            }
            else if (string.Compare(_normalizedDocumentType, "stig", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                continue;
            }
            else if (string.Compare(_normalizedDocumentType, "rule", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                continue;
            }
            else
            {
                throw new InvalidDataException($"The file [{_file.FullName}] references a type [{_normalizedDocumentType}] that is unhandled. Modify the file type and re-run.");
            }
        }
        return;
    }

    /// <summary>
    /// Import the information about the contributors from a json file
    /// </summary>
    public void ImportContributors()
    {
        if (null == ScribeSettings.ContributorsJsonPath)
        {
            throw new InvalidDataException("The ScribeSettings for this Scribe does not contain the directory information for ContributorJsonPath. Add the Path to the JSON file in ScribeSettings.");
        }
        string jsonString = File.ReadAllText(ScribeSettings.ContributorsJsonPath.FullName);
        Contributors = JsonSerializer.Deserialize<List<Contributor>>(jsonString);
        return;
    }
    /// <summary>
    /// Save the current Contributors status in memory to disk
    /// </summary>
    /// <returns>The location of the current JSON File</returns>
    public void SaveContributors()
    {
        _ = ExportContributors(ScribeSettings.ContributorsJsonPath);
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
        return;
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
    public string DocumentType;
}