using Microsoft.Extensions.Configuration;

namespace LV;

internal static class ArgumentExtensions
{
    private static T? GetValueOrDefault<T>(this IConfiguration? config, string key, T? defaultValue)
    {
        if (config is null)
            return defaultValue;

        var value = config[key];
        return string.IsNullOrEmpty(value) ? defaultValue : (T)Convert.ChangeType(value, typeof(T));
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
            OutputEngineType = defaults.GetValueOrDefault("OutputEngineType", defaultOutputEngineType),
            PresentationPath = defaults.GetValueOrDefault("PresentationPath", string.Empty),
            SkipOutput = defaults.GetValueOrDefault("SkipOutput", false),
            SourceRepoPath = Path.GetFullPath(defaults.GetValueOrDefault("SourceRepoPath", defaultSourceRepoPath)),
            SourceRepoType = defaults.GetValueOrDefault("SourceRepoType", defaultSourceRepoType),
            TemplatePath = Path.GetFullPath(defaults.GetValueOrDefault("TemplatePath", defaultTemplatePath))
        };

        for (int i = 0; i < args.Length; i++)
        {
            // TODO: Add any other arguments
            string arg = args[i].ToLower();
            if (arg == "--notitle")
                config.BuildTitleSlide = false;
            else if (arg == "--makesoloimagesfullscreen")
                config.MakeSoloImagesFullScreen = true;
            else if (arg == "--skipoutput")
                config.SkipOutput = true;
            else if (arg.StartsWith("-slidedeckid:"))
                config.SlideDeckId = Guid.Parse(args[i][13..]);
            else if (arg.StartsWith("-slideid:"))
                config.SlideId = Guid.Parse(args[i][9..]);
            else if (arg.StartsWith("-contentitemid:"))
                config.ContentItemId = Guid.Parse(args[i][15..]);
            else if (arg.StartsWith("-sourcerepotype:"))
                config.SourceRepoType = args[i][16..];
            else if (arg.StartsWith("-sourcerepopath:"))
                config.SourceRepoPath = Path.GetFullPath(args[i][16..]);
            else if (arg.StartsWith("-outputenginetype:"))
                config.OutputEngineType = args[i][18..];
            else if (arg.StartsWith("-templatepath:"))
                config.TemplatePath = Path.GetFullPath(args[i][14..]);
            else if (arg.StartsWith("-presentationpath:"))
                config.PresentationPath = Path.GetFullPath(args[i][18..]);
            else if (arg.StartsWith("-title:"))
                config.Title = args[i][7..];
            else if (arg.StartsWith("-contentpath:"))
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
