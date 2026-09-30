namespace LVImport;

internal sealed class ImportOptions
{
    public const string DefaultRepositoryType = "YamlFile";

    public string PowerpointPath { get; set; } = string.Empty;

    public string SourceRepoType { get; set; } = DefaultRepositoryType;

    /// <summary>
    /// The folder path (YamlFile) or connection string (Postgres) of the target repository
    /// </summary>
    public string SourceRepoPath { get; set; } = string.Empty;

    /// <summary>
    /// The 1-based numbers of the slides to import. If empty, all slides are imported.
    /// </summary>
    public IReadOnlyCollection<int> SlideNumbers { get; set; } = [];

    /// <summary>
    /// Overrides the title of the imported slide deck when specified
    /// </summary>
    public string Title { get; set; } = string.Empty;

    public bool SkipOutput { get; set; }

    public bool Verbose { get; set; }

    public bool ShowHelp { get; set; }
}
