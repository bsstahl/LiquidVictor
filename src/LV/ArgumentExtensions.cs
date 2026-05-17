using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace LV;

internal static class ArgumentExtensions
{
    private static T? GetValueOrDefault<T>(this IConfiguration? config, string key, T? defaultValue)
    {
        if (config is null)
            return defaultValue;

        var value = config[key];
        return string.IsNullOrEmpty(value) ? defaultValue : (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
    }

    internal static (Command, Configuration) Parse(this string[] args, IConfiguration? defaults = null)
    {
        const string defaultSourceRepoPath = @"..\..\..\..\..\Sample\Input\";
        const string defaultTemplatePath = @"..\..\..\..\..\Templates\RevealJS\";
        const string defaultOutputEngineType = "RevealJS";
        const string defaultSourceRepoType = "jsonFileSystem";

        var command = Command.Help;
        var config = new Configuration()
        {
            BuildTitleSlide = defaults.GetValueOrDefault("BuildTitleSlide", true),
            MakeSoloImagesFullScreen = defaults.GetValueOrDefault("MakeSoloImagesFullScreen", false),
            OutputEngineType = defaults.GetValueOrDefault("OutputEngineType", defaultOutputEngineType) ?? throw new ArgumentNullException("No argument or default found for OutputEngineType", new InvalidOperationException()),
            PresentationPath = defaults.GetValueOrDefault("PresentationPath", string.Empty) ?? throw new ArgumentNullException("No argument or default found for PresentationPath", new InvalidOperationException()),
            SkipOutput = defaults.GetValueOrDefault("SkipOutput", false),
            SourceRepoPath = Path.GetFullPath(defaults.GetValueOrDefault("SourceRepoPath", defaultSourceRepoPath) ?? throw new ArgumentNullException("No argument or default found for SourceRepoPath", new InvalidOperationException())),
            SourceRepoType = defaults.GetValueOrDefault("SourceRepoType", defaultSourceRepoType) ?? throw new ArgumentNullException("No argument or default found for SourceRepoType", new InvalidOperationException()),
            TemplatePath = Path.GetFullPath(defaults.GetValueOrDefault("TemplatePath", defaultTemplatePath) ?? throw new ArgumentNullException("No argument or default found for TemplatePath", new InvalidOperationException()))
        };

        for (int i = 0; i < args.Length; i++)
        {
            // TODO: Add any other arguments
            string arg = args[i].ToUpperInvariant();
            if (arg == "--NOTITLE")
                config.BuildTitleSlide = false;
            else if (arg == "--MAKESOLOIMAGESFULLSCREEN")
                config.MakeSoloImagesFullScreen = true;
            else if (arg == "--SKIPOUTPUT")
                config.SkipOutput = true;
            else if (arg.StartsWith("-SLIDEDECKID:", StringComparison.Ordinal))
                config.SlideDeckId = Guid.Parse(args[i][13..]);
            else if (arg.StartsWith("-SLIDEID:", StringComparison.Ordinal))
                config.SlideId = Guid.Parse(args[i][9..]);
            else if (arg.StartsWith("-CONTENTITEMID:", StringComparison.Ordinal))
                config.ContentItemId = Guid.Parse(args[i][15..]);
            else if (arg.StartsWith("-SOURCEREPOTYPE:", StringComparison.Ordinal))
                config.SourceRepoType = args[i][16..];
            else if (arg.StartsWith("-SOURCEREPOPATH:", StringComparison.Ordinal))
                config.SourceRepoPath = Path.GetFullPath(args[i][16..]);
            else if (arg.StartsWith("-OUTPUTENGINETYPE:", StringComparison.Ordinal))
                config.OutputEngineType = args[i][18..];
            else if (arg.StartsWith("-TEMPLATEPATH:", StringComparison.Ordinal))
                config.TemplatePath = Path.GetFullPath(args[i][14..]);
            else if (arg.StartsWith("-PRESENTATIONPATH:", StringComparison.Ordinal))
                config.PresentationPath = Path.GetFullPath(args[i][18..]);
            else if (arg.StartsWith("-TITLE:", StringComparison.Ordinal))
                config.Title = args[i][7..];
            else if (arg.StartsWith("-CONTENTPATH:", StringComparison.Ordinal))
                config.ContentPath = Path.GetFullPath(args[i][13..]);
            else
            {
                if (Enum.TryParse<Command>(args[i], true, out var commandResult))
                    command = commandResult;
                else
                    Console.WriteLine($"Unknown parameter '{args[i]}'");
            }
        }

        return (command, config);
    }

    internal static void ValidateNotNullOrEmpty(this string item, string message)
    {
        if (string.IsNullOrWhiteSpace(item))
            throw new ArgumentException(message);
    }

    internal static void ValidateNotNullOrEmpty(this Guid item, string message)
    {
        if (item == Guid.Empty)
            throw new ArgumentException(message);
    }

}
