namespace LiquidVictor.Entities;

public class Resource
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "Other";
#pragma warning disable CA1056 // Resource URLs are serialized and authored as free-text strings.
    public string Url { get; set; } = string.Empty;
#pragma warning restore CA1056
}
