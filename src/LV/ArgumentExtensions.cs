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
        internal static (Command, Configuration) Parse(this string[] args)
        {
            var config = new Configuration();
            var command = Command.Build;

            for (int i = 6; i < args.Length; i++)
            {
                // TODO: Add any other arguments
                string arg = args[i].ToLower();
                if (arg == "--notitle")
                    config.BuildTitleSlide = false;
                else if (arg == "--makesoloimagesfullscreen")
                    config.MakeSoloImagesFullScreen = true;
                else if (arg == "--skipoutput")
                    config.SkipOutput = true;
                else
                {
                    if (Enum.TryParse<Command>(args[i], out var commandResult))
                        command = commandResult;
                }
            }

            return (command, config);
        }
    }
}
