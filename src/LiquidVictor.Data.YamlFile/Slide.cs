using YamlDotNet.Serialization;

namespace LiquidVictor.Data.YamlFile;

internal class Slide
{
    public string Title { get; set; } = string.Empty;
    public string Layout { get; set; } = string.Empty;
    public string TransitionIn { get; set; } = string.Empty;
    public string TransitionOut { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string BackgroundContent { get; set; } = string.Empty;
    public bool NeverFullScreen { get; set; }
    public ChildId[] ContentItemIds { get; set; } = [];


    public override string ToString()
    {
        var serializer = new SerializerBuilder()
            .WithTypeConverter(new ChildIdYamlConverter())
            .Build();
        return serializer.Serialize(this);
    }

    public static Slide Parse(string yaml)
    {
        var deserializer = new DeserializerBuilder()
            .WithTypeConverter(new ChildIdYamlConverter())
            .Build();
        return deserializer.Deserialize<Slide>(yaml);

    }
}
