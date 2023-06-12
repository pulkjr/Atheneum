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
///   <para>Display the current settings for AtheneumPS</para>
///   <code>Get-AtheneumPsSettings</code>
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
            ErrorRecord errorRecord = new(new DirectoryNotFoundException("AtheneumPS settings directory not present. Please run Set-AtheneumPsSettings."), "DirectoryNotFound", ErrorCategory.ReadError, settingsPath);
            WriteError(errorRecord);
            return;
        }
        FileInfo settingsJsonPath = new(Path.Combine(settingsPath.FullName, "AtheneumPsSettings.json"));

        if (!settingsJsonPath.Exists)
        {
            ErrorRecord errorRecord = new(new FileNotFoundException("AtheneumPSSettings.json not present. Please run Set-AtheneumPsSettings."), "JsonFileNotFound", ErrorCategory.ReadError, settingsJsonPath);
            WriteError(errorRecord);
            return;
        }
        ScribeSettings _globalSettings = (ScribeSettings)this.SessionState.PSVariable.GetValue("AtheniumSettings") ?? ImportPsSettingsJson(settingsJsonPath);

        _globalSettings.SettingsJsonPath = settingsJsonPath;

        WriteObject(_globalSettings);
    }
    private ScribeSettings ImportPsSettingsJson(FileInfo jsonPath)
    {
        string jsonString = File.ReadAllText(jsonPath.FullName);

        return JsonSerializer.Deserialize<ScribeSettings>(jsonString);
    }
}