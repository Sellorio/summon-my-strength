using System;

namespace SummonMyStrength.Api.PowerSystems.Items;

public class WikiLinkAttribute : Attribute
{
    public string WikiLink { get; }

    public WikiLinkAttribute(string wikiLink)
    {
        WikiLink = wikiLink;
    }
}
