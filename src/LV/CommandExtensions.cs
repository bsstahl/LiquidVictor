using LiquidVictor.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LV
{
    internal static class CommandExtensions
    {
        internal static void Execute(this Command command, IServiceProvider services, Configuration config)
        {
            var engine = services.GetRequiredService<ICommandEngine>();
            switch (command)
            {
                case Command.Help:
                    Console.WriteLine(engine.GetHelp());
                    break;

                case Command.Build:
                    config.SlideDeckId.ValidateNotNullOrEmpty("A valid Slide Deck must be specified");
                    config.PresentationPath.ValidateNotNullOrEmpty("A valid Presentation Path must be specified");
                    engine.BuildPresentation(config.SlideDeckId);
                    break;

                //case Command.CreateSlideDeck:
                //    config.ExecuteCreateSlideDeck(writeRepo);
                //    break;

                //case Command.CreateSlide:
                //    config.ExecuteCreateSlide(writeRepo);
                //    break;

                //case Command.CreateContentItem:
                //    config.ExecuteCreateContentItem(writeRepo);
                //    break;

                //case Command.ExportContentItem:
                //    config.ExecuteExportContentItem(readRepo, writeRepo);
                //    break;

                //case Command.CloneSlideDeck:
                //    config.ExecuteCloneSlideDeck(readRepo, writeRepo);
                //    break;

                //case Command.CloneSlide:
                //    config.ExecuteCloneSlide(readRepo, writeRepo);
                //    break;

                //case Command.CloneContentItem:
                //    config.ExecuteCloneContentItem(readRepo, writeRepo);
                //    break;

                //case Command.ValidateSourceRepo:
                //    config.ExecuteValidateSourceRepo(readRepo);
                //    break;

                //case Command.FindOrphans:
                //    config.ExecuteFindOrphans(readRepo);
                //    break;

                default:
                    throw new NotImplementedException($"The '{command}' feature has not yet been implemented");
            }

        }
    }
}
