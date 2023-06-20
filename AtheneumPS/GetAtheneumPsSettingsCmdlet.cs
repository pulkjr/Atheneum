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
public class GetAtheneumPsSettingsCmdlet : Base
{
    protected override void EndProcessing()
    {
        ScribeSettings _globalSettings = base.GetSettings();

        WriteObject(_globalSettings);
    }

}