using LiquidVictor.Interfaces;
using System;
using System.Text;

namespace LiquidVictor.Business;

public class Engine : ICommandEngine
{
    readonly ISlideDeckReadRepository _readRepo;
    readonly ISlideDeckWriteRepository _writeRepo;
    readonly IPresentationBuilder _presentationBuilder;

    public Engine(ISlideDeckReadRepository readRepo, ISlideDeckWriteRepository writeRepo, IPresentationBuilder presentationBuilder)
    {
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _presentationBuilder = presentationBuilder;
    }

    public string BuildPresentation(Guid slideDeckId, bool skipOutput)
    {
        // Load a slide deck from a source repository
        // and build it into a RevealJS presentation
        StringBuilder results = new();

        var slideDeck = _readRepo.GetSlideDeck(slideDeckId);
        if (skipOutput)
        {
            engine.CompilePresentation(slideDeck);
            Console.WriteLine($"Presentation '{slideDeck.Title}' successfully compiled");
        }
        else
        {
            engine.CreatePresentation(config.PresentationPath, slideDeck);
            Console.WriteLine($"Presentation '{slideDeck.Title}' written to {config.PresentationPath}");
        }
    }

    public string GetHelp()
    {
        throw new NotImplementedException();
    }
}
