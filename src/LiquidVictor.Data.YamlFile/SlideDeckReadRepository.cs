using LiquidVictor.Exceptions;
using LiquidVictor.Extensions;

namespace LiquidVictor.Data.YamlFile;

public class SlideDeckReadRepository : Interfaces.ISlideDeckReadRepository
{
    readonly string _repositoryPath;
    readonly string _slideDeckPath;
    readonly string _includeBlocksPath;
    readonly string _slidesPath;
    readonly string _contentItemsPath;

    public SlideDeckReadRepository(string repositoryPath)
    {
        _repositoryPath = repositoryPath;
        _slideDeckPath = System.IO.Path.Combine(_repositoryPath, "SlideDecks");
        _includeBlocksPath = System.IO.Path.Combine(_repositoryPath, "IncludeBlocks");
        _contentItemsPath = System.IO.Path.Combine(_repositoryPath, "ContentItems");
        _slidesPath = System.IO.Path.Combine(_repositoryPath, "Slides");
    }

    public IEnumerable<Guid> GetSlideDeckIds()
    {
        return _slideDeckPath.GetFileIds();
    }

    public IEnumerable<Guid> GetSlideIds()
    {
        return _slidesPath.GetFileIds();
    }

    public IEnumerable<Guid> GetContentItemIds()
    {
        return _contentItemsPath.GetFileIds();
    }

    public Entities.SlideDeck GetSlideDeck(Guid id)
    {
        var existingFileName = _slideDeckPath.FindFileWithId(id);
        if (string.IsNullOrEmpty(existingFileName))
            throw new SlideDeckNotFoundException(id, _slideDeckPath);

        var slideDeckContent = System.IO.File.ReadAllText(existingFileName);
        var slideDeck = SlideDeck.Parse(slideDeckContent);

        var includes = new List<Entities.IncludeBlock>();
        foreach (var includeBlock in slideDeck.Includes)
        {
            if (includeBlock.IncludeType.Equals(Enumerations.IncludeType.Slide.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                // A single slide
                var blockSlide = this.GetSlide(Guid.Parse(includeBlock.Id));
                includes.Add(new Entities.IncludeBlock(blockSlide));
            }
            else
            {
                // TODO: A multi-slide block
                var blockSlides = this.GetIncludeBlock(Guid.Parse(includeBlock.Id)).Slides;
                includes.Add(new Entities.IncludeBlock(blockSlides));
            }
        }

        // TODO: Remove this once all Slide Decks have been converted to includes
        if (includes.Count == 0)
        {
            var slides = new List<KeyValuePair<int, Entities.Slide>>();
            int slideIndex = 0;
#pragma warning disable CS0612 // Type or member is obsolete
            foreach (var slideId in slideDeck.SlideIds)
            {
                var slidePair = this.GetSlidePair(slideIndex, slideId.Id);
                slides.Add(slidePair);
                slideIndex++;
            }
#pragma warning restore CS0612 // Type or member is obsolete
            includes = slides.Select(s => new Entities.IncludeBlock(s.Value)).ToList();
        }

        var aspectRatio = Enum.Parse<Enumerations.AspectRatio>(slideDeck.AspectRatio);
        var slideDeckId = Guid.Parse(slideDeck.Id);
        var slideDeckTransition = slideDeck.GetTransition();
        Uri slideDeckUri = string.IsNullOrWhiteSpace(slideDeck.SlideDeckUrl) ? new Uri("about:blank") : new Uri(slideDeck.SlideDeckUrl);

        var result = new Entities.SlideDeck(slideDeckId, slideDeck.Title, slideDeck.SubTitle, slideDeck.Presenter, slideDeck.ThemeName, slideDeckUri, slideDeck.PrintLinkText, slideDeckTransition, aspectRatio, includes.OrderBy(i => 0));

        return result;
    }

    private KeyValuePair<int, Entities.ContentItem> GetContentItemPair(int contentItemIndex, Guid contentItemId)
    {
        return new KeyValuePair<int, Entities.ContentItem>(contentItemIndex, this.GetContentItem(contentItemId));
    }

    private KeyValuePair<int, Entities.Slide> GetSlidePair(int slideIndex, Guid slideId)
    {
        return new KeyValuePair<int, Entities.Slide>(slideIndex, this.GetSlide(slideId));
    }

    public Entities.Slide GetSlide(Guid id)
    {
        var slidePath = System.IO.Path.Combine(_slidesPath, $"{id}.yaml");
        var slideContent = System.IO.File.ReadAllText(slidePath);
        var slide = Slide.Parse(slideContent);

        int contentItemIndex = 0;
        var contentItems = new List<KeyValuePair<int, Entities.ContentItem>>();
        foreach (var contentItemId in slide.ContentItemIds)
        {
            try
            {
                var contentItemPair = this.GetContentItemPair(contentItemIndex, contentItemId.Id);
                contentItems.Add(contentItemPair);
                contentItemIndex++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to load Slide {id} due to error: '{ex.Message}'.");
                throw;
            }
        }

        Guid? backgroundContentItemId = null;
        if (Guid.TryParse(slide.BackgroundContent, out var parsedValue))
            backgroundContentItemId = parsedValue;

        var slideResult = new Entities.Slide()
        {
            Id = id,
            Layout = Enum.Parse<Enumerations.Layout>(slide.Layout),
            Notes = slide.Notes,
            Title = slide.Title,
            TransitionIn = Enum.Parse<Enumerations.Transition>(slide.TransitionIn),
            TransitionOut = Enum.Parse<Enumerations.Transition>(slide.TransitionOut),
            BackgroundContent = backgroundContentItemId.HasValue ? this.GetContentItem(backgroundContentItemId.Value) : null,
            NeverFullScreen = slide.NeverFullScreen
        };

        contentItems.ForEach(ci => slideResult.ContentItems.Add(ci));

        return slideResult;
    }

    public Entities.ContentItem GetContentItem(Guid id)
    {
        var contentItemPath = System.IO.Path.Combine(_contentItemsPath, $"{id}.yaml");
        var contentItemContent = System.IO.File.ReadAllText(contentItemPath);
        var localContentItem = ContentItem.Parse(contentItemContent);

        var contentItemEntity = new Entities.ContentItem()
        {
            Id = id,
            Alignment = localContentItem.Alignment,
            ContentType = localContentItem.ContentType,
            FileName = localContentItem.FileName,
            Title = localContentItem.Title,
            Content = localContentItem.EncodedContent.DecodeContent(localContentItem.ContentType)
        };

        return contentItemEntity;
    }

    public IEnumerable<Entities.SlideDeck> GetSlideDecks()
    {
        throw new NotImplementedException();
        //var result = new List<Entities.SlideDeck>();
        //var slideDeckIds = this.GetSlideDeckIds();
        //foreach (var slideDeckId in slideDeckIds)
        //{
        //    result.Add(this.GetSlideDeck(slideDeckId));
        //}

        //return result;
    }

    public IEnumerable<Entities.Slide> GetSlides()
    {
        var result = new List<Entities.Slide>();
        var slideIds = this.GetSlideIds();
        foreach (var slideId in slideIds)
        {
            result.Add(this.GetSlide(slideId));
        }
        return result;
    }

    public IEnumerable<Entities.ContentItem> GetContentItems()
    {
        var result = new List<Entities.ContentItem>();
        var contentItemIds = this.GetContentItemIds();
        foreach (var contentItemId in contentItemIds)
        {
            result.Add(this.GetContentItem(contentItemId));
        }
        return result;
    }

    internal static (IEnumerable<Guid> SlideDeckIds, IEnumerable<Guid> SlideIds, IEnumerable<Guid> ContentItemIds) FindDuplicateIds(IEnumerable<Entities.SlideDeck> slideDecks)
    {
#pragma warning disable CA1851 // Possible multiple enumerations of 'IEnumerable' collection
        var slides = slideDecks.SelectMany(d => d.Slides).Select(s => s.Value);
        var contentItems = slides.SelectMany(s => s.ContentItems).Select(c => c.Value);

        var deckIds = slideDecks
            .GroupBy(d => d.Id)
            .Select(g => new { Id = g.Key, Count = g.Count() })
            .Where(c => c.Count > 1)
            .ToList();

        var slideIds = slides
            .GroupBy(s => s.Id)
            .Select(g => new { Id = g.Key, Count = g.Count() })
            .Where(c => c.Count > 1)
            .ToList();

        var contentItemIds = contentItems
            .GroupBy(c => c.Id)
            .Select(g => new { Id = g.Key, Count = g.Count() })
            .Where(c => c.Count > 1)
            .ToList();
#pragma warning restore CA1851 // Possible multiple enumerations of 'IEnumerable' collection

        return (Array.Empty<Guid>(), Array.Empty<Guid>(), Array.Empty<Guid>());
    }

    public IEnumerable<Guid> GetIncludeBlockIds() => throw new NotImplementedException();
    
    public Entities.IncludeBlock GetIncludeBlock(Guid id)
    {
        var includeBlockPath = Path.Combine(_includeBlocksPath, $"{id}.yaml");
        var includeBlockContent = File.ReadAllText(includeBlockPath);
        var localIncludeBlock = IncludeBlock.Parse(id.ToString(), includeBlockContent);

        var includedSlides = localIncludeBlock.SlideIds.Select(i => this.GetSlide(Guid.Parse(i))).OrderBy(i => 0);
        return new Entities.IncludeBlock(includedSlides);
    }

    public IEnumerable<Entities.IncludeBlock> GetIncludeBlocks() => throw new NotImplementedException();
}
