using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Text.Json;
using Atheneum;

namespace AtheneumPS;

/// <summary>
/// <para type="synopsis">Display the Global Settings</para>
/// </summary>
/// <example>
///   <para>Create a new article for a Brocade KB Article</para>
///   <code>New-AnArticle -Title "Create New FCP Alias" -Type Create -Technology Brocade -Path "c:\scripts\git\storage\site\docs\brocade"</code>
/// </example>
[Cmdlet(VerbsCommon.Get, "AtheneumPsSettings")]
[Alias("Get-AnModuleSettings")]
[OutputType(typeof(ScribeSettings))]
public class GetAtheneumPsSettingsCmdlet : PSCmdlet
{
    protected override void EndProcessing()
    {
        DirectoryInfo settingsPath = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Atheneum"));

        if (!settingsPath.Exists)
        {
            throw new CmdletInvocationException("AtheneumPS settings directory not present. Please run Set-AtheneumPsSettings.");
        }
        FileInfo settingsJsonPath = new(Path.Combine(settingsPath.FullName, "AtheneumPsSettings.json"));

        if (!settingsJsonPath.Exists)
        {
            throw new CmdletInvocationException("AtheneumPSSettings.json not present. Please run Set-AtheneumPsSettings.");
        }
        ScribeSettings _globalSettings = (ScribeSettings)this.SessionState.PSVariable.GetValue("AtheniumSettings") ?? ImportPsSettingsJson(settingsJsonPath);

        _globalSettings.SettingsJsonPath = settingsJsonPath;

        WriteObject(_globalSettings);
    }
    private ScribeSettings ImportPsSettingsJson(FileInfo jsonPath)
    {
        if (!jsonPath.Exists)
        {
            throw new CmdletInvocationException("AtheneumPSSettings.json not present, nothing to import. Please run Set-AtheneumPsSettings.");
        }
        string jsonString = File.ReadAllText(jsonPath.FullName);

        return JsonSerializer.Deserialize<ScribeSettings>(jsonString);
    }
}