using Microsoft.Extensions.Logging;

namespace LVImport;

internal static partial class Log
{
    [LoggerMessage(EventId = 100, Level = LogLevel.Information, Message = "Starting PowerPoint import of {PowerpointPath}")]
    public static partial void ImportStarting(ILogger logger, string powerpointPath);

    [LoggerMessage(EventId = 101, Level = LogLevel.Information, Message = "Saving slide deck {SlideDeckId} with {SlideCount} slide(s) to the {RepositoryType} repository")]
    public static partial void SavingSlideDeck(ILogger logger, Guid slideDeckId, int slideCount, string repositoryType);

    [LoggerMessage(EventId = 102, Level = LogLevel.Information, Message = "Saved slide deck {SlideDeckId} to the {RepositoryType} repository")]
    public static partial void SlideDeckSaved(ILogger logger, Guid slideDeckId, string repositoryType);

    [LoggerMessage(EventId = 103, Level = LogLevel.Information, Message = "Output skipped; slide deck {SlideDeckId} was imported but not saved")]
    public static partial void SaveSkipped(ILogger logger, Guid slideDeckId);

    [LoggerMessage(EventId = 110, Level = LogLevel.Trace, Message = "Import options: repositoryType={RepositoryType}, slides={SlideNumbers}, title={Title}, skipOutput={SkipOutput}")]
    public static partial void ImportOptions(ILogger logger, string repositoryType, IEnumerable<int> slideNumbers, string title, bool skipOutput);

    [LoggerMessage(EventId = 111, Level = LogLevel.Trace, Message = "Overriding title of slide deck {SlideDeckId} with {Title}")]
    public static partial void TitleOverridden(ILogger logger, Guid slideDeckId, string title);

    [LoggerMessage(EventId = 120, Level = LogLevel.Error, Message = "PowerPoint import of {PowerpointPath} failed")]
    public static partial void ImportFailed(ILogger logger, string powerpointPath, Exception exception);
}
