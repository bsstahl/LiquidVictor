using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LV
{
    internal static class ArgumentExtensions
    {
        internal static (Command, Configuration) Parse(this string[] args, Configuration defaults = null)
        {
            var command = Command.Build;
            var config = defaults ?? new Configuration()
            {
                BuildTitleSlide = true,
                MakeSoloImagesFullScreen = false,
                OutputEngineType = "RevealJS",
                PresentationPath = System.IO.Path.GetFullPath(@"..\..\..\..\..\Sample\Output\"),
                SkipOutput = false,
                SourceRepoPath = System.IO.Path.GetFullPath(@"..\..\..\..\..\Sample\Input\"),
                SourceRepoType = "jsonFileSystem",
                TemplatePath = System.IO.Path.GetFullPath(@"..\..\..\..\..\Templates\RevealJS\")
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
                    config.TemplatePath = System.IO.Path.GetFullPath(args[i].Substring(18));
                else if (arg.StartsWith("-presentationpath:"))
                    config.PresentationPath = System.IO.Path.GetFullPath(args[i].Substring(18));
                else if (arg.StartsWith("-title:"))
                    config.Title = args[i].Substring(7);
                else
                {
                    if (Enum.TryParse<Command>(args[i], out var commandResult))
                        command = commandResult;
                    else
                        Console.WriteLine($"Unknown parameter '{args[i]}'");
                }
            }

            return (command, config);
        }
    }
}
