using Microsoft.Extensions.Configuration;
using System;
using System.Threading;

namespace LV
{
    internal static class ArgumentExtensions
    {
        internal static (Command, Configuration) Parse(this string[] args, IConfiguration defaults = null)
        {
            const string defaultSourceRepoPath = @"..\..\..\..\..\Sample\Input\";
            const string defaultTemplatePath = @"..\..\..\..\..\Templates\RevealJS\";
            const string defaultOutputEngineType = "RevealJS";
            const string defaultSourceRepoType = "jsonFileSystem";

            var command = Command.Help;
            var config = new Configuration()
            {
                BuildTitleSlide = defaults?.GetValue<bool>("BuildTitleSlide") ?? true,
                MakeSoloImagesFullScreen = defaults?.GetValue<bool>("MakeSoloImagesFullScreen") ?? false,
                OutputEngineType = defaults?.GetValue<string>("OutputEngineType") ?? defaultOutputEngineType,
                PresentationPath = defaults?.GetValue<string>("PresentationPath") ?? string.Empty,
                SkipOutput = defaults?.GetValue<bool>("SkipOutput") ?? false,
                SourceRepoPath = System.IO.Path.GetFullPath(defaults?.GetValue<string>("SourceRepoPath") ?? defaultSourceRepoPath),
                SourceRepoType = defaults?.GetValue<string>("SourceRepoType") ?? defaultSourceRepoType,
                TemplatePath = System.IO.Path.GetFullPath(defaults?.GetValue<string>("TemplatePath") ?? defaultTemplatePath)
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
                    config.SlideDeckId = Guid.Parse(args[i].Substring(13));
                else if (arg.StartsWith("-slideid:"))
                    config.SlideId = Guid.Parse(args[i].Substring(9));
                else if (arg.StartsWith("-contentitemid:"))
                    config.ContentItemId = Guid.Parse(args[i].Substring(15));
                else if (arg.StartsWith("-sourcerepotype:"))
                    config.SourceRepoType = args[i].Substring(16);
                else if (arg.StartsWith("-sourcerepopath:"))
                    config.SourceRepoPath = System.IO.Path.GetFullPath(args[i].Substring(16));
                else if (arg.StartsWith("-outputenginetype:"))
                    config.OutputEngineType = args[i].Substring(18);
                else if (arg.StartsWith("-templatepath:"))
                    config.TemplatePath = System.IO.Path.GetFullPath(args[i].Substring(14));
                else if (arg.StartsWith("-presentationpath:"))
                    config.PresentationPath = System.IO.Path.GetFullPath(args[i].Substring(18));
                else if (arg.StartsWith("-title:"))
                    config.Title = args[i].Substring(7);
                else if (arg.StartsWith("-contentpath:"))
                    config.ContentPath = System.IO.Path.GetFullPath(args[i].Substring(13));
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
}
