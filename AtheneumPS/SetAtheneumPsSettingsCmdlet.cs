using System;
using System.IO;
using System.Management.Automation;
using System.Text.Json;
using System.Text.Json.Serialization;
using Atheneum;

namespace AtheneumPS;

/// <summary>
/// <para type="synopsis">Configure the Global Settings</para>
/// </summary>
/// <example>
///   <para>Create a new article for a Brocade KB Article</para>
///   <code>New-AnArticle -Title "Create New FCP Alias" -Type Create -Technology Brocade -Path "c:\scripts\git\storage\site\docs\brocade"</code>
/// </example>
[Cmdlet(VerbsCommon.Set, "AtheneumPsSettings")]
[Alias("Set-AnModuleSettings")]
[OutputType(typeof(Article))]
public class SetAtheneumPsSettingsCmdlet : PSCmdlet
{
    /// <summary>
    /// <para type="description">The Path to the documentation parent directory.</para>
    /// </summary>
    [Parameter(Mandatory = true)]
    public DirectoryInfo DocumentationDirectory;

    /// <summary>
    /// <para type="description">The Path to the JSON File where the Contributors information is stored.</para>
    /// </summary>
    [Parameter()]
    [AllowNull()]
    public FileInfo? ContributorsJsonPath;
    /// <summary>
    /// <para type="description">The Path to the JSON File where the Contributors information is stored.</para>
    /// </summary>
    [Parameter()]
    [AllowNull()]
    public DirectoryInfo? MarkdownTemplateDirectory;

    protected async override void EndProcessing()
    {
        DirectoryInfo settingsPath = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Atheneum"));

        FileInfo settingsJsonPath = new(Path.Combine(settingsPath.FullName, "AtheneumPsSettings.json"));

        GetAtheneumPsSettingsCmdlet _getPsSettings = new();

        ScribeSettings _globalSettings;
        try
        {
            _globalSettings = (ScribeSettings)_getPsSettings.Invoke();
        }
        catch
        {
            if (!settingsPath.Exists)
            {
                settingsPath.Create();
            }

            _globalSettings = new ScribeSettings(settingsJsonPath, DocumentationDirectory, ContributorsJsonPath);
        }

        _globalSettings.DocumentationDirectory = DocumentationDirectory;

        _globalSettings.ContributorsJsonPath = ContributorsJsonPath;

        _globalSettings.MarkdownTemplateDirectory = MarkdownTemplateDirectory;

        try
        {
            _globalSettings.Save();
        }
        catch (Exception e)
        {
            throw e;
        }

        SessionState.PSVariable.Set(new PSVariable("AtheniumSettings", _globalSettings, ScopedItemOptions.Private));
    }
}