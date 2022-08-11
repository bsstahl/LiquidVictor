using LiquidVictor.Interfaces;
using System;

namespace LV
{
    internal static class CommandExtensions
    {
        internal static void Execute(this Command command, Configuration config, ISlideDeckReadRepository readRepo, ISlideDeckWriteRepository writeRepo, IPresentationBuilder engine)
        {
            switch (command)
            {
                case Command.Help:
                    config.ExecuteHelp();
                    break;

                case Command.Build:
                    config.ExecuteBuild(readRepo, engine);
                    break;

                case Command.CreateSlideDeck:
                    config.ExecuteCreateSlideDeck(writeRepo);
                    break;

                case Command.CreateSlide:
                    config.ExecuteCreateSlide(writeRepo);
                    break;

                case Command.CreateContentItem:
                    config.ExecuteCreateContentItem(writeRepo);
                    break;

                case Command.ExportContentItem:
                    config.ExecuteExportContentItem(readRepo, writeRepo);
                    break;

                case Command.CloneSlideDeck:
                    config.ExecuteCloneSlideDeck(readRepo, writeRepo);
                    break;

                case Command.CloneSlide:
                    config.ExecuteCloneSlide(readRepo, writeRepo);
                    break;

                case Command.CloneContentItem:
                    config.ExecuteCloneContentItem(readRepo, writeRepo);
                    break;

                case Command.ValidateSourceRepo:
                    config.ExecuteValidateSourceRepo(readRepo);
                    break;

                case Command.FindOrphans:
                    config.ExecuteFindOrphans(readRepo);
                    break;

                default:
                    throw new NotImplementedException($"The '{command}' feature has not yet been implemented");
            }

        }
    }
}
