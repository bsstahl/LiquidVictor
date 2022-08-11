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
                case Command.Help: // TODO: Implement Help command
                    throw new NotImplementedException("Help command not yet implemented");

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

                case Command.ExportContentItem: // TODO: Implement ExportContentItem command
                    throw new NotImplementedException("ExportContentItem command not yet implemented");

                case Command.CloneSlideDeck:
                    config.ExecuteCloneSlideDeck(readRepo, writeRepo);
                    break;

                case Command.CloneSlide:
                    config.ExecuteCloneSlide(readRepo, writeRepo);
                    break;

                case Command.CloneContentItem: // TODO: Implement CloneContentItem command
                    throw new NotImplementedException("CloneContentItem command not yet implemented");

                case Command.ValidateSourceRepo: // TODO: Implement ValidateSourceRepo command
                    config.ExecuteValidateSourceRepo(readRepo);
                    break;

                case Command.FindOrphans: // TODO: Implement FindOrphans command
                    throw new NotImplementedException();

                default:
                    throw new NotImplementedException($"The '{command}' feature has not yet been implemented");
            }

        }
    }
}
