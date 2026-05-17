using LiquidVictor.Enumerations;
using YamlDotNet.Serialization;

namespace LiquidVictor.Data.YamlFile;

internal class SlideDeck
{
    const Transition _defaultTransition = Enumerations.Transition.Slide;
    const Format _defaultFormat = Enumerations.Format.Session;

    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string SubTitle { get; set; } = string.Empty;
    public string Presenter { get; set; } = string.Empty;
    public string ThemeName { get; set; } = string.Empty;
    public string AspectRatio { get; set; } = string.Empty;
    public string PrintLinkText { get; set; } = string.Empty;
    public string Transition { get; set; } = string.Empty;
    public string SlideDeckUrl { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;

    [Obsolete]
    public ChildId[] SlideIds { get; set; } = [];
    public Include[] Includes { get; set; } = [];


    internal Transition GetTransition() => Enum.TryParse<Enumerations.Transition>(this.Transition, out var result) 
        ? result  : _defaultTransition;

    internal Format GetFormat() => Enum.TryParse<Enumerations.Format>(this.Format, out var result) 
        ? result : _defaultFormat;

    public static SlideDeck Parse(string yaml)
    {
        var deserializer = new DeserializerBuilder()
            .WithTypeConverter(new ChildIdYamlConverter())
            .Build();
        return deserializer.Deserialize<SlideDeck>(yaml);
    }

    public override string ToString()
    {
        var serializer = new SerializerBuilder()
            .WithTypeConverter(new ChildIdYamlConverter())
            .Build();
        return serializer.Serialize(this);
    }
}
