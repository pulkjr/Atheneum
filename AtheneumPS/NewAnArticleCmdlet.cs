#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Atheneum;
using Atheneum.Enums;

namespace AtheneumPS;

/// <summary>
/// <para type="synopsis">Create a new Atheneum Article markdown file.</para>
/// </summary>
/// <example>
///   <para>Create a new article for a Brocade KB Article</para>
///   <code>New-AnArticle -Title "Create New FCP Alias" -Type Create -Technology Brocade -Path "c:\scripts\git\storage\site\docs\brocade"</code>
/// </example>
[Cmdlet(VerbsCommon.New, "AnArticle")]
[Alias("New-PdocArticle")]
[OutputType(typeof(Article))]
public class NewAnArticleCmdlet : Base
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// <para type="description">The Name of the article</para>
    /// </summary>
    [Parameter(Mandatory = true)]
    [Alias("Name")]
    [ValidateNotNullOrEmpty()]
    public string Title;

    /// <summary>
    /// <para type="description">The type of article</para>
    /// </summary>
    [Parameter(Mandatory = true)]
    [ValidateNotNullOrEmpty()]
    public ArticleType Type;

    /// <summary>
    /// <para type="description">The technology that the article is grouped into</para>
    /// </summary>
    [Parameter(Mandatory = true)]
    [ValidateNotNullOrEmpty()]
    public string Technology;

    /// <summary>
    /// <para type="description">The path to the file this object is referencing on disk</para>
    /// </summary>
    [Parameter(Mandatory = false)]
    public DirectoryInfo Path;

    /// <summary>
    /// <para type="description">The list of authors who wrote this article</para>
    /// </summary>
    [Parameter()]
    public List<string> Author;

    /// <summary>
    /// <para type="description">The difficulty of this article to complete</para>
    /// </summary>
    [Parameter()]
    public Difficulty? Difficulty;

    /// <summary>
    /// <para type="description">The skillset of the individual executing this article</para>
    /// </summary>
    [Parameter()]
    public Skillset? Skillset;

    /// <summary>
    /// <para type="description">The abstract describing the article content in short form</para>
    /// </summary>
    [Parameter()]
    public string? Abstract;

    /// <summary>
    /// <para type="description">Is this article published?</para>
    /// </summary>
    [Parameter()]
    public bool DisablePublication = false;

    /// <summary>
    /// <para type="description">The keywords that describe this article</para>
    /// </summary>
    [Parameter()]
    public List<string>? Keywords;

    /// <summary>
    /// <para type="description">The section that this article is grouped with</para>
    /// </summary>
    [Parameter()]
    public string? Section;

    /// <summary>
    /// <para type="description">The order in which this will show on the site</para>
    /// </summary>
    [Parameter()]
    public Int16? Order;

    /// <summary>
    /// <para type="description">Date that the metadata was verified</para>
    /// </summary>
    [Parameter()]
    public DateTime VerifiedMetaData;

    /// <summary>
    /// <para type="description">Date that the content was verified</para>
    /// </summary>
    [Parameter()]
    public DateTime VerifiedContent;

    /// <summary>
    /// <para type="description">Subject Matter Experts for this article</para>
    /// </summary>
    [Parameter()]
    public List<string> SME;

    /// <summary>
    /// <para type="description">Is this activity considered Impacting?</para>
    /// </summary>
    [Parameter()]
    public bool IsImpacting = false;

    /// <summary>
    /// <para type="description">Is this article a training topic?</para>
    /// </summary>
    [Parameter()]
    public bool IsTrainingTopic = false;

    /// <summary>
    /// <para type="description">How often should this article be trained?</para>
    /// </summary>
    [Parameter()]
    public TrainingFrequency? TrainingFrequency;

    /// <summary>
    /// <para type="description">Link to a referenced article or KM</para>
    /// </summary>
    [Parameter()]
    public List<ReferenceLink>? References;

    /// <summary>
    /// <para type="description">Link to a km ID</para>
    /// </summary>
    [Parameter()]
    public string? KmId;

    /// <summary>
    /// <para type="description">The last date this article was synced to ITSM Knowledge Base</para>
    /// </summary>
    [Parameter()]
    public DateTime KmLastSync;

    /// <summary>
    /// <para type="description">The date that training was last conducted for this article</para>
    /// </summary>
    [Parameter()]
    public DateTime LastTrained;

    /// <summary>
    /// <para type="description">The software version this article has been tested for</para>
    /// </summary>
    [Parameter()]
    public List<string> SoftwareVersion;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override void BeginProcessing()
    {
    }
    protected override void ProcessRecord()
    {
        ScribeSettings scribeSettings = base.GetSettings();

        if (null == scribeSettings)
        {
            throw new Exception("The PS Settings are null");
        }
        Article article = new();
        article.Title = Title;
        article.Type = Type;
        string name = GetNameFromTitle();

        DirectoryInfo _technologyDirectory = new(System.IO.Path.Combine(scribeSettings.DocumentationDirectory.FullName, Technology));

        if (!_technologyDirectory.Exists)
        {
            _technologyDirectory.Create();
        }
        article.Path = new FileInfo(System.IO.Path.Combine(_technologyDirectory.FullName, name));
        
        article.Technology = Technology;

        FileInfo templateFileInfo = new(System.IO.Path.Combine(scribeSettings.MarkdownTemplateDirectory.FullName, $"{Type}.template.md"));

        string markdownString = "<!-- TODO:LOW:  [Backlog] - Add Content -->";

        if (templateFileInfo.Exists)
        {
            markdownString = File.ReadAllText(templateFileInfo.FullName);
        }
        article.SetMarkdown(markdownString);

        // Lists
        if (Author != null && Author.Count >= 0)
        {
            article.Author = Author;
        }
        if (Keywords != null && Keywords.Count >= 0)
        {
            article.Keywords = Keywords;
        }
        if (SME != null && SME.Count >= 0)
        {
            article.SME = SME;
        }
        if (References != null && References.Count >= 0)
        {
            article.References = References;
        }
        if (SoftwareVersion != null && SoftwareVersion.Count >= 0)
        {
            article.SoftwareVersion = SoftwareVersion;
        }

        // enums
        if (Difficulty != null)
        {
            article.Difficulty = Difficulty.Value;
        }
        if (Skillset != null)
        {
            article.Skillset = Skillset.Value;
        }
        if (TrainingFrequency != null)
        {
            article.TrainingFrequency = TrainingFrequency.Value;
        }

        // bool
        article.DisablePublication = DisablePublication;
        article.IsImpacting = IsImpacting;
        article.IsTrainingTopic = IsTrainingTopic;


        // Strings
        article.Abstract ??= Abstract;
        article.Section ??= Section;
        article.KmId ??= KmId;

        // int
        if (Order != null && Order != 0)
        {
            article.Order = Order;
        }

        // DateTime
        if (VerifiedMetaData != null && VerifiedMetaData != default(DateTime))
        {
            article.VerifiedMetaData = VerifiedMetaData;
        }
        if (VerifiedContent != null && VerifiedContent != default(DateTime))
        {
            article.VerifiedContent = VerifiedContent;
        }
        if (KmLastSync != null && KmLastSync != default(DateTime))
        {
            article.KmLastSync = KmLastSync;
        }
        if (LastTrained != null && LastTrained != default(DateTime))
        {
            article.LastTrained = LastTrained;
        }
        article.Save();
        WriteObject(article);
    }

    private string GetNameFromTitle()
    {
        string name = $"{Type} - {Title}.md";
        if (Order != null && Order != 0)
        {
            if (string.IsNullOrEmpty(Section))
            {
                name = $"{Type}.{Section}.{Order} - {Title}.md";
            }
            else
            {
                name = $"{Type}.{Order} - {Title}.md";
            }
        }
        else if (!string.IsNullOrEmpty(Section))
        {
            name = $"{Type}.{Section} - {Title}.md";
        }
        return name;
    }
}