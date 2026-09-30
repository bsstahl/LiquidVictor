using LiquidVictor.Entities;
using LiquidVictor.Input.Powerpoint;
using LiquidVictor.Interfaces;
using Microsoft.Extensions.Logging;

namespace LVImport;

internal sealed class ImportCommand
{
    private readonly SlideDeckImporter _importer;
    private readonly ILogger<ImportCommand> _logger;

    public ImportCommand(SlideDeckImporter importer, ILogger<ImportCommand> logger)
    {
        ArgumentNullException.ThrowIfNull(importer);
        ArgumentNullException.ThrowIfNull(logger);
        _importer = importer;
        _logger = logger;
    }

    /// <summary>
    /// Imports the PowerPoint deck described by the options and saves it to the repository
    /// </summary>
    /// <param name="options">The import options</param>
    /// <param name="writeRepository">The target repository. Not used (and may be null) when <see cref="ImportOptions.SkipOutput"/> is set.</param>
    public SlideDeck Execute(ImportOptions options, ISlideDeckWriteRepository? writeRepository)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (!options.SkipOutput)
            ArgumentNullException.ThrowIfNull(writeRepository);

        Log.ImportStarting(_logger, options.PowerpointPath);
        Log.ImportOptions(_logger, options.SourceRepoType, options.SlideNumbers, options.Title, options.SkipOutput);

        var slideDeck = _importer.Import(options.PowerpointPath, options.SlideNumbers);

        if (!string.IsNullOrWhiteSpace(options.Title))
        {
            Log.TitleOverridden(_logger, slideDeck.Id, options.Title);
            slideDeck.Title = options.Title;
        }

        if (options.SkipOutput)
            Log.SaveSkipped(_logger, slideDeck.Id);
        else
        {
            Log.SavingSlideDeck(_logger, slideDeck.Id, slideDeck.Slides.Count, options.SourceRepoType);
            writeRepository!.SaveSlideDeck(slideDeck);
            Log.SlideDeckSaved(_logger, slideDeck.Id, options.SourceRepoType);
        }

        return slideDeck;
    }
}
