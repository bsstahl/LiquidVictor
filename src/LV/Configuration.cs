namespace LV;

sealed internal class Configuration
{
    public bool BuildTitleSlide { get; set; } = true;

    public bool MakeSoloImagesFullScreen { get; set; }

    public bool SkipOutput { get; set; }

    public Guid SlideDeckId { get; set; } = Guid.Empty;

    public Guid SlideId { get; set; } = Guid.Empty;

    public Guid ContentItemId { get; set; } = Guid.Empty;

    public string SourceRepoType { get; set; } = string.Empty;

    public string SourceRepoPath { get; set; } = string.Empty;

    public string OutputEngineType { get; set; } = string.Empty;

    public string TemplatePath { get; set; } = string.Empty;

    public string PresentationPath { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string ContentPath { get; set; } = string.Empty;

}
