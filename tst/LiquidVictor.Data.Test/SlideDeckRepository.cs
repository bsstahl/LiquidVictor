using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Exceptions;
using LiquidVictor.Interfaces;
using LiquidVictor.Enumerations;

namespace LiquidVictor.Data.Hardcoded;

public class SlideDeckRepository : ISlideDeckReadRepository
{
    private readonly IReadOnlyDictionary<Guid, SlideDeck> _slideDecks;
    private readonly IReadOnlyDictionary<Guid, Slide> _slides;
    private readonly IReadOnlyDictionary<Guid, ContentItem> _contentItems;
    private readonly IReadOnlyDictionary<Guid, IncludeBlock> _includeBlocks;

    public SlideDeckRepository()
        : this(CreateDemoData())
    { }

    public SlideDeckRepository(
        IEnumerable<SlideDeck> slideDecks,
        IEnumerable<Slide>? slides = null,
        IEnumerable<ContentItem>? contentItems = null,
        IEnumerable<KeyValuePair<Guid, IncludeBlock>>? includeBlocks = null)
    {
        ArgumentNullException.ThrowIfNull(slideDecks);

        var deckList = slideDecks.ToList();
        var slideList = (slides ?? deckList.SelectMany(deck => deck.Slides).Select(pair => pair.Value))
            .DistinctBy(slide => slide.Id)
            .ToList();
        var contentItemList = (contentItems ?? GetDeckContentItems(deckList, slideList))
            .DistinctBy(contentItem => contentItem.Id)
            .ToList();
        var includeBlockList = includeBlocks
            ?? slideList.Select(slide => new KeyValuePair<Guid, IncludeBlock>(slide.Id, new IncludeBlock(slide)));

        _slideDecks = deckList.ToDictionary(deck => deck.Id);
        _slides = slideList.ToDictionary(slide => slide.Id);
        _contentItems = contentItemList.ToDictionary(contentItem => contentItem.Id);
        _includeBlocks = includeBlockList.ToDictionary(includeBlock => includeBlock.Key, includeBlock => includeBlock.Value);
    }

    private SlideDeckRepository(DemoData data)
        : this(data.SlideDecks, includeBlocks: data.IncludeBlocks)
    { }

    public IEnumerable<SlideDeck> GetSlideDecks() => _slideDecks.Values;

    public SlideDeck GetSlideDeck(Guid id)
    {
        if (_slideDecks.TryGetValue(id, out var slideDeck))
            return slideDeck;

        throw new SlideDeckNotFoundException(id, nameof(SlideDeckRepository));
    }

    public IEnumerable<Guid> GetSlideDeckIds() => _slideDecks.Keys;

    public IEnumerable<Slide> GetSlides() => _slides.Values;

    public Slide GetSlide(Guid id) => GetById(_slides, id, "slide");

    public IEnumerable<Guid> GetSlideIds() => _slides.Keys;

    public IEnumerable<ContentItem> GetContentItems() => _contentItems.Values;

    public ContentItem GetContentItem(Guid id) => GetById(_contentItems, id, "content item");

    public IEnumerable<Guid> GetContentItemIds() => _contentItems.Keys;

    public IEnumerable<IncludeBlock> GetIncludeBlocks() => _includeBlocks.Values;

    public IncludeBlock GetIncludeBlock(Guid id) => GetById(_includeBlocks, id, "include block");

    public IEnumerable<Guid> GetIncludeBlockIds() => _includeBlocks.Keys;

    private static TEntity GetById<TEntity>(
        IReadOnlyDictionary<Guid, TEntity> entities,
        Guid id,
        string entityName)
    {
        if (entities.TryGetValue(id, out var entity))
            return entity;

        throw new KeyNotFoundException($"Unable to find {entityName} with ID {id}.");
    }

    private static IEnumerable<ContentItem> GetDeckContentItems(
        IEnumerable<SlideDeck> slideDecks,
        IEnumerable<Slide> slides)
    {
        return slideDecks
            .Where(deck => deck.BackgroundContent is not null)
            .Select(deck => deck.BackgroundContent!)
            .Concat(slides.SelectMany(slide => slide.ContentItems.Select(pair => pair.Value)))
            .Concat(slides.Where(slide => slide.BackgroundContent is not null).Select(slide => slide.BackgroundContent!));
    }

    private static DemoData CreateDemoData()
    {
        var background = CreateContentItem(
            "10000000-0000-0000-0000-000000000001",
            "image/png",
            "background.png",
            "Presentation Background",
            "center",
            ["demo", "background"],
            "background image");
        var bulletPoints = CreateContentItem(
            "10000000-0000-0000-0000-000000000002",
            "text/markdown",
            "bullet-points.md",
            "Bullet Points",
            "left",
            ["demo", "bullets"],
            "* Bullet Point 1\r\n* Bullet Point 2");
        var paragraph = CreateContentItem(
            "10000000-0000-0000-0000-000000000003",
            "text/plain",
            "paragraph.txt",
            "Paragraph",
            "left",
            ["demo", "paragraph"],
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce facilisis consectetur dui ac ultrices.");
        var rightText = CreateContentItem(
            "10000000-0000-0000-0000-000000000004",
            "text/markdown",
            "image-right.md",
            "Image Right Text",
            "left",
            ["demo", "image-right"],
            "* Bullet Point 3\r\n* Bullet Point 4");
        var rightImage = CreateContentItem(
            "10000000-0000-0000-0000-000000000005",
            "image/svg",
            "Angles2.svg",
            "Angles",
            "right",
            ["demo", "image"],
            "<svg xmlns=\"http://www.w3.org/2000/svg\" />");
        var leftText = CreateContentItem(
            "10000000-0000-0000-0000-000000000006",
            "text/markdown",
            "image-left.md",
            "Image Left Text",
            "right",
            ["demo", "image-left"],
            "* Bullet Point 5\r\n* Bullet Point 6\r\n* Bullet Point 7\r\n* Bullet Point 8\r\n* Bullet Point 9\r\n* Bullet Point 10");
        var leftImage = CreateContentItem(
            "10000000-0000-0000-0000-000000000007",
            "image/png",
            "System States.png",
            "System States",
            "left",
            ["demo", "image"],
            "system states image");

        var fullScreen = CreateSlide(
            "20000000-0000-0000-0000-000000000001",
            "Full Screen Slide",
            Layout.FullPage,
            [bulletPoints],
            background,
            neverFullScreen: false);
        var paragraphSlide = CreateSlide(
            "20000000-0000-0000-0000-000000000002",
            "Paragraph Slide",
            Layout.FullPageFragments,
            [paragraph],
            neverFullScreen: true);
        var imageRight = CreateSlide(
            "20000000-0000-0000-0000-000000000003",
            "Image-Right Slide",
            Layout.ImageRight,
            [rightText, rightImage],
            background,
            neverFullScreen: false);
        var imageLeft = CreateSlide(
            "20000000-0000-0000-0000-000000000004",
            "Image-Left Slide",
            Layout.ImageLeft,
            [leftText, leftImage],
            neverFullScreen: true);

        var singleSlideBlocks = new[]
        {
            CreateIncludeBlock("30000000-0000-0000-0000-000000000001", fullScreen),
            CreateIncludeBlock("30000000-0000-0000-0000-000000000002", paragraphSlide),
            CreateIncludeBlock("30000000-0000-0000-0000-000000000003", imageRight),
            CreateIncludeBlock("30000000-0000-0000-0000-000000000004", imageLeft)
        };
        var groupedSlideBlocks = new[]
        {
            CreateIncludeBlock("30000000-0000-0000-0000-000000000005", paragraphSlide, fullScreen),
            CreateIncludeBlock("30000000-0000-0000-0000-000000000006", imageLeft, imageRight)
        };
        var mixedSlideBlocks = new[]
        {
            CreateIncludeBlock("30000000-0000-0000-0000-000000000007", imageRight),
            CreateIncludeBlock("30000000-0000-0000-0000-000000000008", fullScreen, imageLeft),
            CreateIncludeBlock("30000000-0000-0000-0000-000000000009", paragraphSlide)
        };

        var slideDecks = new[]
        {
            CreateSlideDeck(
                "E0B187D2-C9B7-4635-8FE5-0CA21BC5007F",
                "Demo Presentation",
                "A Liquid Victor Demonstration",
                "Joe Presenter (@joep)",
                "black",
                new Uri("https://example.com/demo"),
                Transition.Slide,
                AspectRatio.Widescreen,
                Format.Session,
                background,
                singleSlideBlocks.Select(block => block.Value)),
            CreateSlideDeck(
                "728f2f58-9ee6-4e5a-9281-26d094b7a68a",
                "Demo Presentation",
                "A Liquid Victor Demonstration",
                "Joe Presenter (@joep)",
                "moon",
                new Uri("https://example.com/demo/grouped"),
                Transition.Fade,
                AspectRatio.Standard,
                Format.Talk,
                background,
                groupedSlideBlocks.Select(block => block.Value)),
            CreateSlideDeck(
                "bcf43e23-4771-4862-baf7-9bccb9c096c5",
                "Demo Presentation",
                "A Liquid Victor Demonstration",
                "Joe Presenter (@joep)",
                "carvana",
                new Uri("https://example.com/demo/mixed"),
                Transition.Fancy,
                AspectRatio.Widescreen,
                Format.ShortWorkshop,
                background,
                mixedSlideBlocks.Select(block => block.Value))
        };

        return new DemoData(slideDecks, singleSlideBlocks.Concat(groupedSlideBlocks).Concat(mixedSlideBlocks));
    }

    private static ContentItem CreateContentItem(
        string id,
        string contentType,
        string fileName,
        string title,
        string alignment,
        IEnumerable<string> tags,
        string content)
    {
        return new ContentItem(
            Guid.Parse(id),
            Encoding.UTF8.GetBytes(content),
            contentType,
            fileName,
            title,
            alignment,
            tags);
    }

    private static Slide CreateSlide(
        string id,
        string title,
        Layout layout,
        IEnumerable<ContentItem> contentItems,
        ContentItem? backgroundContent = null,
        bool neverFullScreen = false)
    {
        var pairs = contentItems
            .Select((contentItem, index) => new KeyValuePair<int, ContentItem>((index + 1) * 10, contentItem))
            .ToList();

        return new Slide(
            Guid.Parse(id),
            title,
            layout,
            Transition.PresentationDefault,
            Transition.PresentationDefault,
            $"Notes for {title}",
            backgroundContent,
            neverFullScreen,
            pairs)
        {
            BackgroundTransitionIn = Transition.Fade,
            BackgroundTransitionOut = Transition.Slide
        };
    }

    private static KeyValuePair<Guid, IncludeBlock> CreateIncludeBlock(string id, params Slide[] slides)
    {
        return new KeyValuePair<Guid, IncludeBlock>(
            Guid.Parse(id),
            new IncludeBlock(slides.OrderBy(_ => 0)));
    }

    private static SlideDeck CreateSlideDeck(
        string id,
        string title,
        string subTitle,
        string presenter,
        string themeName,
        Uri slideDeckUrl,
        Transition transition,
        AspectRatio aspectRatio,
        Format format,
        ContentItem backgroundContent,
        IEnumerable<IncludeBlock> includeBlocks)
    {
        return new SlideDeck(
            Guid.Parse(id),
            title,
            subTitle,
            presenter,
            themeName,
            slideDeckUrl,
            "Printable Version",
            transition,
            aspectRatio,
            format,
            includeBlocks.OrderBy(_ => 0))
        {
            BackgroundTransition = Transition.Fade,
            BackgroundContent = backgroundContent
        };
    }

    private sealed record DemoData(
        IEnumerable<SlideDeck> SlideDecks,
        IEnumerable<KeyValuePair<Guid, IncludeBlock>> IncludeBlocks);
}
