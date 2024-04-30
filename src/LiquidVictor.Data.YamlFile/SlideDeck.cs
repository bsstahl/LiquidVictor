using LiquidVictor.Enumerations;
using System;
using YamlDotNet.Serialization;

namespace LiquidVictor.Data.YamlFile;

internal class SlideDeck
{
    const Transition _defaultTransition = Enumerations.Transition.Slide;
    const Format _defaultFormat = Enumerations.Format.Session;

    public string Id { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string Presenter { get; set; }
    public string ThemeName { get; set; }
    public string AspectRatio { get; set; }
    public string PrintLinkText { get; set; }
    public string Transition { get; set; }
    public string SlideDeckUrl { get; set; }
    public string Format { get; set; }
    public ChildId[] SlideIds { get; set; }

    internal Transition GetTransition()
    {
        Transition result = _defaultTransition;
        Enum.TryParse<Enumerations.Transition>(this.Transition, out result);
        return result;
    }

    internal Format GetFormat()
    {
        Format result = _defaultFormat;
        Enum.TryParse<Enumerations.Format>(this.Format, out result);
        return result;
    }

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
