#nullable enable
using System;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Atheneum.Enums;
using Markdig;
using Markdig.Syntax;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Extensions.Yaml;
using Markdig.Renderers.Normalize;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace Atheneum;

/// <summary>
/// A process or knowledge article
/// </summary>
public class Article
{
    /// <summary>
    /// The Name of the article
    /// </summary>
    [Required(ErrorMessage = "The property 'title' is retuired")]
    [YamlMember(Alias = "title")]
    public string Title;

    /// <summary>
    /// The type of article 
    /// </summary>
    [Required]
    [YamlMember(Alias = "type")]
    public ArticleType Type;

    /// <summary>
    /// The technology that the article is grouped into
    /// </summary>
    [Required]
    [YamlMember(Alias = "technology")]
    public string Technology;

    /// <summary>
    /// The list of authors who wrote this article
    /// </summary>
    [YamlMember(Alias = "author")]
    public List<string> Author = new();

    /// <summary>
    /// The difficulty of this article to complete
    /// </summary>
    [YamlMember(Alias = "difficulty")]
    public Difficulty Difficulty;

    /// <summary>
    /// The skillset of the individual executing this article
    /// </summary>
    [YamlMember(Alias = "skillset")]
    public Skillset Skillset;

    /// <summary>
    /// The abstract describing the article content in short form
    /// </summary>
    [YamlMember(Alias = "abstract", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public string? Abstract;

    /// <summary>
    /// The ID of this article
    /// </summary>
    [YamlMember(Alias = "id")]
    public string ID = System.Guid.NewGuid().ToString();

    /// <summary>
    /// Is this article published?
    /// </summary>
    [YamlMember(Alias = "disable-publication", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public bool DisablePublication = false;

    /// <summary>
    /// The keywords that describe this article
    /// </summary>
    [YamlMember(Alias = "keywords", DefaultValuesHandling = DefaultValuesHandling.OmitEmptyCollections)]
    public List<string> Keywords = new();

    /// <summary>
    /// The section that this article is grouped with
    /// </summary>
    [YamlMember(Alias = "section")]
    public string? Section;

    /// <summary>
    /// The order in which this will show on the site
    /// </summary>
    [YamlMember(Alias = "order")]
    public Int16? Order;

    /// <summary>
    /// Date that the metadata was verified
    /// </summary>
    [YamlMember(Alias = "verified-metadata", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public DateTime VerifiedMetaData;

    /// <summary>
    /// Date that the content was verified
    /// </summary>
    [YamlMember(Alias = "verified-content", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public DateTime VerifiedContent;

    /// <summary>
    /// Subject Matter Experts for this article
    /// </summary>
    [YamlMember(Alias = "sme", DefaultValuesHandling = DefaultValuesHandling.OmitEmptyCollections)]
    public List<string> SME = new();

    /// <summary>
    /// Is this activity considered Impacting?
    /// </summary>
    [YamlMember(Alias = "is-impacting", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public bool IsImpacting = false;

    /// <summary>
    /// Is this article a training topic?
    /// </summary>
    [YamlMember(Alias = "training-topic", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public bool IsTrainingTopic = false;

    /// <summary>
    /// How often should this article be trained?
    /// </summary>
    [YamlMember(Alias = "training-frequency")]
    public TrainingFrequency? TrainingFrequency;

    /// <summary>
    /// Link to a referenced article or KM
    /// </summary>
    [YamlMember(Alias = "references", DefaultValuesHandling = DefaultValuesHandling.OmitEmptyCollections)]
    public List<ReferenceLink>? References = new();

    /// <summary>
    /// Link to a km ID
    /// </summary>
    [YamlMember(Alias = "km-id", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public string? KmId;

    /// <summary>
    /// The last date this article was synced to ITSM Knowledge Base
    /// </summary>
    [YamlMember(Alias = "km-last-sync", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public DateTime? KmLastSync;

    /// <summary>
    /// Does this article require syncronization with the ITSM?
    /// </summary>
    [YamlIgnore]
    public bool IsKmSyncRequired
    {
        get
        {
            if (!string.IsNullOrEmpty(KmId))
            {
                if (KmLastSync != null && DateTime.Now.AddYears(-1) > KmLastSync)
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// The date that training was last conducted for this article
    /// </summary>
    [YamlMember(Alias = "last-trained", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public DateTime LastTrained;

    /// <summary>
    /// Does this article require training based on the <see cref="LastTrained"/> Date and the <see cref="TrainingFrequency"/>
    /// </summary>
    [YamlIgnore]
    public bool IsTrainingRequired
    {
        get
        {
            if (IsTrainingTopic)
            {
                if (TrainingFrequency != null && TrainingFrequency == Enums.TrainingFrequency.Yearly && DateTime.Now.AddYears(-1) > LastTrained)
                {
                    return true;
                }
                else if (TrainingFrequency != null && TrainingFrequency == Enums.TrainingFrequency.Quarterly && DateTime.Now.AddMonths(-3) > LastTrained)
                {
                    return true;
                }
                else if (TrainingFrequency != null && TrainingFrequency == Enums.TrainingFrequency.SemiAnnually && DateTime.Now.AddMonths(-6) > LastTrained)
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// The software version this article has been tested for
    /// </summary>
    [YamlMember(Alias = "software-version", DefaultValuesHandling = DefaultValuesHandling.OmitEmptyCollections)]
    public List<string> SoftwareVersion = new();

    /// <summary>
    /// The path to the file this object is referencing on disk
    /// </summary>
    [YamlIgnore]
    public FileInfo Path;

    /// <summary>
    /// The Markdown document itself.
    /// </summary>
    [YamlIgnore]
    public MarkdownDocument Markdown;


    /// <summary>
    /// Initialize a new instance of an <see cref="Article"/> class.
    /// </summary>
    public Article() { }

    public void ToHtml(string Path)
    {
        StringWriter writer = new();

        // Create a HTML Renderer and setup it with the pipeline
        HtmlRenderer renderer = new(writer);

        Atheneum.Pipeline.Setup(renderer);

        // Renders markdown to HTML (to the writer)
        _ = renderer.Render(Markdown);

        // Gets the rendered string
        File.WriteAllText(Path, writer.ToString());
    }

    public override string ToString()
    {
        return Title;
    }

    /// <summary>
    /// Save this article to disk.
    /// </summary>
    public void Save()
    {
        StringBuilder _articleString = new();
        _articleString.AppendLine("---");
        var serializer = new SerializerBuilder()
            .WithIndentedSequences()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();
        serializer.Serialize(new IndentedTextWriter(new StringWriter(_articleString)), this);
        _articleString.AppendLine("---");


        // Found this here: https://github.com/xoofx/markdig/issues/155
        using var stringWriter = new StringWriter();
        var renderer = new NormalizeRenderer(stringWriter);
        Atheneum.Pipeline.Setup(renderer);
        renderer.Render(Markdown);
        _articleString.AppendLine(stringWriter.ToString());

        File.WriteAllText(Path.FullName, _articleString.ToString());
    }
}
