using LiquidVictor.Enumerations;
using System;

namespace LiquidVictor.Data.JsonFileSystem;

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
    public string FileName {  get; set; }
    public string Format { get; set; }
    public string[] SlideIds { get; set; }

    internal Transition GetTransition()
    {
        Transition result = _defaultTransition;
        Enum.TryParse<Enumerations.Transition>(this.Transition, out result);
        return result;
    }

    internal Format GetFormat()
    {
        Format result = _defaultFormat;
        Enum.TryParse<Format>(this.Format, out result);
        return result;
    }
}
