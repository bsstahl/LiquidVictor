namespace LiquidVictor.Output.RevealJs.Entities;

public class BuilderOptions
{
    public bool BuildTitleSlide { get; set; } = true;

    public bool MakeSoloImagesFullScreen { get; set; } = false;

    public Enumerations.Format Format { get; set; } = Enumerations.Format.AllSlides;
}
