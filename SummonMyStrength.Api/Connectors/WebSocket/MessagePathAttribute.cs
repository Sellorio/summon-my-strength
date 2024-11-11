using System;

namespace SummonMyStrength.Api.Connectors.WebSocket;

[AttributeUsage(AttributeTargets.Field)]
public class MessagePathAttribute : Attribute
{
    public string Path { get; }
    public bool StartsWith { get; set; }

    public MessagePathAttribute(string path)
    {
        Path = path;
    }
}
