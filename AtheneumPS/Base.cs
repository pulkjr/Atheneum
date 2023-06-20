using System;
using System.IO;
using System.Management.Automation;
using Atheneum;
using System.Text.Json;

namespace AtheneumPS;
public class Base : PSCmdlet
{
    public ScribeSettings GetSettings()
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

        return _globalSettings;
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