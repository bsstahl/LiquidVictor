using Microsoft.Extensions.Logging;

namespace LiquidVictor.Output.RemarkJs.Generator;

internal static partial class Log
{
    [LoggerMessage(EventId = 100, Level = LogLevel.Information, Message = "Compiling Remark.js presentation")]
    internal static partial void CompilationStarted(ILogger logger);

    [LoggerMessage(EventId = 105, Level = LogLevel.Trace, Message = "Remark.js presentation title: {PresentationTitle}")]
    internal static partial void CompilationDetails(ILogger logger, string presentationTitle);

    [LoggerMessage(EventId = 110, Level = LogLevel.Information, Message = "Compiled Remark.js presentation")]
    internal static partial void CompilationCompleted(ILogger logger);

    [LoggerMessage(EventId = 115, Level = LogLevel.Error, Message = "Failed to compile Remark.js presentation")]
    internal static partial void CompilationFailed(ILogger logger, Exception exception);

    [LoggerMessage(EventId = 120, Level = LogLevel.Information, Message = "Generating Remark.js presentation")]
    internal static partial void GenerationStarted(ILogger logger);

    [LoggerMessage(EventId = 125, Level = LogLevel.Trace, Message = "Remark.js presentation title={PresentationTitle}, outputPath={PresentationPath}")]
    internal static partial void GenerationDetails(ILogger logger, string presentationTitle, string presentationPath);

    [LoggerMessage(EventId = 130, Level = LogLevel.Information, Message = "Generated Remark.js presentation with {SlideCount} slide(s) and {ImageCount} image(s)")]
    internal static partial void GenerationCompleted(ILogger logger, int slideCount, int imageCount);

    [LoggerMessage(EventId = 135, Level = LogLevel.Error, Message = "Failed to generate Remark.js presentation")]
    internal static partial void GenerationFailed(ILogger logger, Exception exception);
}
