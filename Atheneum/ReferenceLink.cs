using System;
using YamlDotNet.Serialization;

namespace Atheneum;
public class ReferenceLink
{
    // The title of the referenced article or km ID.
    [YamlMember(Alias = "title")]
    public string Title;

    // The link to the referenced material
    [YamlMember(typeof(string), Alias = "link")]
    public Uri Link;

    public ReferenceLink()
    {
    }
}