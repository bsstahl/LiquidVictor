using System;

namespace LiquidVictor.Interfaces;

public interface ICommandEngine
{
    string GetHelp();
    string BuildPresentation(Guid slideDeckId, string presentationPath);
    string CreateTableOfContents(Guid slideDeckId, string outputFilePath);
}
