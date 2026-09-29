using Microsoft.Extensions.Logging;

namespace LiquidVictor.Input.Powerpoint;

internal static partial class Log
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Importing PowerPoint deck {SourceName}")]
    public static partial void ImportStarting(ILogger logger, string sourceName);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Imported PowerPoint deck {SourceName} as slide deck {SlideDeckId} with {SlideCount} slide(s) and {ContentItemCount} content item(s)")]
    public static partial void ImportCompleted(ILogger logger, string sourceName, Guid slideDeckId, int slideCount, int contentItemCount);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Selected {SelectedCount} of {SlideCount} slide(s) from {SourceName} for import")]
    public static partial void SlidesSelected(ILogger logger, int selectedCount, int slideCount, string sourceName);

    [LoggerMessage(EventId = 10, Level = LogLevel.Trace, Message = "Deck properties for {SourceName}: title={Title}, subTitle={SubTitle}, presenter={Presenter}, aspectRatio={AspectRatio}, sourceTag={SourceTag}")]
    public static partial void DeckProperties(ILogger logger, string sourceName, string title, string subTitle, string presenter, Enumerations.AspectRatio aspectRatio, string sourceTag);

    [LoggerMessage(EventId = 11, Level = LogLevel.Trace, Message = "Imported slide {SlideNumber} titled {SlideTitle} with {TextItemCount} text item(s) and {ImageCount} image(s) using layout {Layout}")]
    public static partial void SlideImported(ILogger logger, int slideNumber, string slideTitle, int textItemCount, int imageCount, Enumerations.Layout layout);

    [LoggerMessage(EventId = 12, Level = LogLevel.Trace, Message = "Imported image {FileName} ({ContentType}, {ByteCount} bytes) from slide {SlideNumber}")]
    public static partial void ImageImported(ILogger logger, string fileName, string contentType, int byteCount, int slideNumber);

    [LoggerMessage(EventId = 20, Level = LogLevel.Warning, Message = "Skipping picture {PictureName} on slide {SlideNumber} because its image data is not embedded in the deck")]
    public static partial void PictureSkipped(ILogger logger, string pictureName, int slideNumber);

    [LoggerMessage(EventId = 21, Level = LogLevel.Warning, Message = "Skipping unsupported element {ElementName} on slide {SlideNumber}; its content will not be imported")]
    public static partial void UnsupportedElementSkipped(ILogger logger, string elementName, int slideNumber);

    [LoggerMessage(EventId = 22, Level = LogLevel.Warning, Message = "Skipping slide {SlideNumber} of {SourceName} because its slide part could not be resolved")]
    public static partial void SlidePartMissing(ILogger logger, int slideNumber, string sourceName);

    [LoggerMessage(EventId = 23, Level = LogLevel.Warning, Message = "Unable to determine the slide size of {SourceName}; defaulting aspect ratio to {AspectRatio}")]
    public static partial void AspectRatioDefaulted(ILogger logger, string sourceName, Enumerations.AspectRatio aspectRatio);

    [LoggerMessage(EventId = 30, Level = LogLevel.Error, Message = "PowerPoint file {FilePath} was not found")]
    public static partial void SourceFileNotFound(ILogger logger, string filePath);

    [LoggerMessage(EventId = 31, Level = LogLevel.Error, Message = "Unable to open {SourceName} as a PowerPoint presentation")]
    public static partial void InvalidPresentation(ILogger logger, string sourceName, Exception? exception);

    [LoggerMessage(EventId = 32, Level = LogLevel.Error, Message = "Requested slide number {SlideNumber} is outside the range 1-{SlideCount} for {SourceName}")]
    public static partial void SlideNumberOutOfRange(ILogger logger, int slideNumber, int slideCount, string sourceName);
}
