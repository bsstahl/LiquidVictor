using YamlDotNet.Serialization;

namespace LiquidVictor.Data.YamlFile;

internal class ContentItem
{
    public string ContentType { get; set; }
    public string FileName { get; set; }
    public string Title { get; set; }
    public string EncodedContent { get; set; }
    public string Alignment { get; set; }

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
