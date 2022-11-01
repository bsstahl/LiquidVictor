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

    public string BuildPresentation(Guid slideDeckId, string presentationPath)
    {
        // Load a slide deck from a source repository
        // and build it into a RevealJS presentation
        StringBuilder results = new();
        var skipOutput = string.IsNullOrWhiteSpace(presentationPath);
        var slideDeck = _readRepo.GetSlideDeck(slideDeckId);
        if (skipOutput)
        {
            _presentationBuilder.CompilePresentation(slideDeck);
            Console.WriteLine($"Presentation '{slideDeck.Title}' successfully compiled");
        }
        else
        {
            _presentationBuilder.CreatePresentation(presentationPath, slideDeck);
            Console.WriteLine($"Presentation '{slideDeck.Title}' written to {presentationPath}");
        }

        // HACK: What should be returned here?
        return String.Empty;
    }

    public string GetHelp()
    {
        throw new NotImplementedException();
    }
}
