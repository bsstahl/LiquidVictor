using LiquidVictor.Extensions;
using LiquidVictor.Interfaces;
using System;
using System.Linq;
using System.Text;

namespace LiquidVictor.Business;

public class Engine : ICommandEngine
{
    readonly IServiceProvider _services;

    public Engine(IServiceProvider services)
    {
        _services = services;
    }

    public string BuildPresentation(Guid slideDeckId, string presentationPath)
    {
        // Load a slide deck from a source repository
        // and build it into a RevealJS presentation
        var readRepo = _services.GetReadRepo();
        var presentationBuilder = _services.GetPresentationBuilder();

        StringBuilder results = new();
        var skipOutput = string.IsNullOrWhiteSpace(presentationPath);

        var slideDeck = readRepo.GetSlideDeck(slideDeckId);
        if (skipOutput)
        {
            presentationBuilder.CompilePresentation(slideDeck);
            Console.WriteLine($"Presentation '{slideDeck.Title}' successfully compiled");
        }
        else
        {
            presentationBuilder.CreatePresentation(presentationPath, slideDeck);
            Console.WriteLine($"Presentation '{slideDeck.Title}' written to {presentationPath}");
        }

        // HACK: What should be returned here?
        return String.Empty;
    }

    public string CreateTableOfContents(Guid slideDeckId, string outputFilePath)
    {
        // Load a slide deck from a source repository and create
        // a list of all slides in the deck in table-of-contents form
        var readRepo = _services.GetReadRepo();
        var tocStrategy = _services.GetTocStrategy();

        var skipOutput = string.IsNullOrWhiteSpace(outputFilePath);
        
        var slideDeck = readRepo.GetSlideDeck(slideDeckId);
        var tocSlide = tocStrategy.GetContentsSlide(slideDeck);

        if (skipOutput)
        {
            var contentDetail = tocSlide.ContentItems.First().Value.Content.AsString();
            Console.WriteLine(contentDetail.Replace("\\r\\n", "\r\n"));
        }
        else
        {
            throw new NotImplementedException();
            // TODO: Write slide to proper repo location
            //System.IO.File.WriteAllText(outputFilePath, tocSlide.ToString());
            //Console.WriteLine($"Table of contents slide for '{slideDeck.Title}' written to {outputFilePath}");
        }

        // HACK: What should be returned here?
        return String.Empty;
    }

    public string GetHelp()
    {
        throw new NotImplementedException();
    }

    public string ConvertRepo()
    {
        throw new NotImplementedException();

        //string outputPath = $"..\\..\\..\\..\\..\\..\\LiquidVictorDatabases\\JsonFileSystem\\IntroToWasmAndBlazor";
        //var source = new LiquidVictor.Data.Postgres.SlideDeckReadRepository();
        //var target = new LiquidVictor.Data.JsonFileSystem.SlideDeckWriteRepository(outputPath);

        //Guid slideDeckId = Guid.Parse("c11b3e5f-1b2a-430c-8be7-b37377c4c198");
        //var slideDeck = source.GetSlideDeck(slideDeckId);
        //target.SaveSlideDeck(slideDeck);
    }
}
