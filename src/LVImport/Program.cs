using LiquidVictor.Exceptions;
using LiquidVictor.Input.Powerpoint;
using LiquidVictor.Interfaces;
using Microsoft.Extensions.Logging;

namespace LVImport;

internal static class Program
{
    private const int _success = 0;
    private const int _failure = 1;

    static int Main(string[] args)
    {
        ImportOptions options;
        try
        {
            options = ImportArguments.Parse(args);
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine();
            Console.Error.WriteLine(Messages.Usage);
            return _failure;
        }

        if (options.ShowHelp)
        {
            Console.WriteLine(Messages.Usage);
            return _success;
        }

        using var loggerFactory = LoggerFactory.Create(builder => builder
            .SetMinimumLevel(options.Verbose ? LogLevel.Trace : LogLevel.Information)
            .AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace)
            .AddSimpleConsole(o => o.SingleLine = true));

        var logger = loggerFactory.CreateLogger(typeof(Program).FullName ?? nameof(Program));
        var command = new ImportCommand(
            new SlideDeckImporter(loggerFactory.CreateLogger<SlideDeckImporter>()),
            loggerFactory.CreateLogger<ImportCommand>());

        ISlideDeckWriteRepository? writeRepository = null;
        try
        {
            if (!options.SkipOutput)
                writeRepository = WriteRepositoryFactory.Create(options.SourceRepoType, options.SourceRepoPath);

            var slideDeck = command.Execute(options, writeRepository);
            Console.WriteLine(options.SkipOutput
                ? $"Slide Deck {slideDeck.Id} ('{slideDeck.Title}') imported with {slideDeck.Slides.Count} slide(s); output skipped"
                : $"Slide Deck {slideDeck.Id} ('{slideDeck.Title}') imported with {slideDeck.Slides.Count} slide(s) and saved to the {options.SourceRepoType} repository");
            return _success;
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException or ArgumentException
            or UnauthorizedAccessException or NotSupportedException or DuplicateEntityIdException)
        {
            Log.ImportFailed(logger, options.PowerpointPath, ex);
            return _failure;
        }
        finally
        {
            (writeRepository as IDisposable)?.Dispose();
        }
    }
}
