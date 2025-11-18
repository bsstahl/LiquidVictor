using YamlDotNet.Serialization;

namespace LiquidVictor.Data.YamlFile;

internal class ContentItem
{
    public string ContentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string EncodedContent { get; set; } = string.Empty;
    public string Alignment { get; set; } = string.Empty;

    public override string ToString()
    {
        var serializer = new SerializerBuilder().Build();
        return serializer.Serialize(this);
    }

    public static ContentItem Parse(string yaml)
    {
        var deserializer = new DeserializerBuilder()
            .Build();
        return deserializer.Deserialize<ContentItem>(yaml);
    }
}
