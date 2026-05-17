using System.Linq;
using LiquidVictor.Exceptions;
using LiquidVictor.Extensions;

namespace LiquidVictor.Data.YamlFile;

public class SlideDeckWriteRepository(string sourceFolderPath) : Interfaces.ISlideDeckWriteRepository
{
    private readonly string _sourceFolderPath = System.IO.Path.GetFullPath(sourceFolderPath);

    public void SaveSlideDeck(Entities.SlideDeck slideDeck)
    {
        ArgumentNullException.ThrowIfNull(slideDeck);
        var duplicates = SlideDeckReadRepository.FindDuplicateIds(new[] { slideDeck });
        if (duplicates.SlideIds.Any())
            throw new DuplicateEntityIdException("Slide", duplicates.SlideIds);
        if (duplicates.ContentItemIds.Any())
            throw new DuplicateEntityIdException("ContentItem", duplicates.ContentItemIds);

        if (!System.IO.Directory.Exists(_sourceFolderPath))
            System.IO.Directory.CreateDirectory(_sourceFolderPath);

        var slideDecksPath = System.IO.Path.Combine(_sourceFolderPath, "SlideDecks");
        if (!System.IO.Directory.Exists(slideDecksPath))
            System.IO.Directory.CreateDirectory(slideDecksPath);

        var sd = new SlideDeck()
        {
            Id = slideDeck.Id.ToString(),
            AspectRatio = slideDeck.AspectRatio.ToString(),
            Presenter = slideDeck.Presenter,
            PrintLinkText = slideDeck.PrintLinkText,
            SubTitle = slideDeck.SubTitle,
            ThemeName = slideDeck.ThemeName,
            Title = slideDeck.Title,
            Transition = slideDeck.Transition.ToString(),
            Format = slideDeck.Format.ToString(),
            SlideDeckUrl = slideDeck.SlideDeckUrl?.ToString() ?? string.Empty,
            Includes = slideDeck.Slides.OrderBy(s => s.Key)
                .Select(s => new Include { Id = s.Value.Id.ToString(), IncludeType = Enumerations.IncludeType.Slide.ToString() })
                .ToArray()
        };

        var slideDeckFileName = GetSlideDeckFileName(slideDeck);
        string slideDeckPath = System.IO.Path.Combine(slideDecksPath, $"{slideDeckFileName}.yaml");

        // Write SlideDeck file
        File.WriteAllText(slideDeckPath, sd.ToString());

        // Write Slides
        // TODO: Deduplicate (in case a slide is used more than once in a presentation)
        foreach (var s in slideDeck.Slides)
            this.SaveSlide(s.Value);
    }

    public void SaveSlide(Entities.Slide slide)
    {
        ArgumentNullException.ThrowIfNull(slide, nameof(slide));

        string slidesPath = System.IO.Path.Combine(_sourceFolderPath, "Slides");
        if (!System.IO.Directory.Exists(slidesPath))
            System.IO.Directory.CreateDirectory(slidesPath);

        var newSlide = new Slide()
        {
            BackgroundContent = slide.BackgroundContent?.Id.ToString() ?? string.Empty,
            Layout = slide.Layout.ToString(),
            NeverFullScreen = slide.NeverFullScreen,
            Notes = slide.Notes,
            Title = slide.Title,
            TransitionIn = slide.TransitionIn.ToString(),
            TransitionOut = slide.TransitionOut.ToString(),
            ContentItemIds = slide.ContentItems.OrderBy(ci => ci.Key).Select(ci => new ChildId(ci.Value.Id, ci.Value.Title)).ToArray()
        };

        // Write slide file
        string slidePath = System.IO.Path.Combine(slidesPath, $"{slide.Id}.yaml");
        File.WriteAllText(slidePath, newSlide.ToString());

        // Write ContentItems
        foreach (var ci in slide.ContentItems)
            this.SaveContentItem(ci.Value);

        if (slide.BackgroundContent is not null)
            this.SaveContentItem(slide.BackgroundContent);
    }

    public void SaveContentItem(Entities.ContentItem contentItem)
    {
        ArgumentNullException.ThrowIfNull(contentItem, nameof(contentItem));

        var ci = new ContentItem()
        {
            Alignment = contentItem.Alignment,
            ContentType = contentItem.ContentType,
            EncodedContent = contentItem.Content.EncodeContent(contentItem.ContentType),
            FileName = contentItem.FileName,
            Title = contentItem.Title
        };

        string contentItemsPath = System.IO.Path.Combine(_sourceFolderPath, "ContentItems");
        if (!System.IO.Directory.Exists(contentItemsPath))
            System.IO.Directory.CreateDirectory(contentItemsPath);

        // Write ContentItem file
        string contentItemPath = System.IO.Path.Combine(contentItemsPath, $"{contentItem.Id}.yaml");
        File.WriteAllText(contentItemPath, ci.ToString());
    }

    private string GetSlideDeckFileName(Entities.SlideDeck slideDeck)
    {
        // If the slide deck already exists (per the id), use that filename
        // otherwise, use a filename generated from the title and format of the presentation
        var slideDecksPath = System.IO.Path.Combine(_sourceFolderPath, "SlideDecks");
        var existingFilePath = System.IO.Directory.Exists(slideDecksPath)
            ? slideDecksPath.FindFileWithId(slideDeck.Id)
            : string.Empty;

        string result;
        if (string.IsNullOrWhiteSpace(existingFilePath))
        {
            var fullTitle = $"{slideDeck.Title}-{slideDeck.SubTitle}".Trim();
            result = $"{fullTitle}-{slideDeck.Format}".Clean();
            var filePath = System.IO.Path.Combine(slideDecksPath, $"{result}.yaml");
            if (File.Exists(filePath))
                throw new InvalidOperationException($"SlideDeck already exists at '{filePath}'");
        }
        else
        {
            result = System.IO.Path.GetFileNameWithoutExtension(existingFilePath);
        }

        return result;
    }

}
