using LiquidVictor.Interfaces;

namespace LVImport;

internal static class WriteRepositoryFactory
{
    private const string _yamlFile = "YAMLFILE";
    private const string _postgres = "POSTGRES";
    private const string _postgresql = "POSTGRESQL";

    internal static bool IsSupported(string repositoryType)
        => repositoryType?.ToUpperInvariant() is _yamlFile or _postgres or _postgresql;

    internal static ISlideDeckWriteRepository Create(string repositoryType, string repositoryPath)
    {
        ArgumentNullException.ThrowIfNull(repositoryType);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositoryPath);

        return repositoryType.ToUpperInvariant() switch
        {
            _yamlFile => new LiquidVictor.Data.YamlFile.SlideDeckWriteRepository(Path.GetFullPath(repositoryPath)),
            _postgres or _postgresql => new LiquidVictor.Data.Postgres.SlideDeckWriteRepository(repositoryPath),
            _ => throw new NotSupportedException($"Invalid Source Repository Type '{repositoryType}'")
        };
    }
}
