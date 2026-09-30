using System.Globalization;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using LiquidVictor.Builders;
using LiquidVictor.Entities;
using LiquidVictor.Enumerations;
using Microsoft.Extensions.Logging;
using D = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

namespace LiquidVictor.Input.Powerpoint;

/// <summary>
/// Converts a PowerPoint (.pptx) presentation into a LiquidVictor <see cref="SlideDeck"/>.
/// Each PowerPoint slide becomes a <see cref="Slide"/>, text shapes become markdown
/// <see cref="ContentItem"/>s, embedded pictures become image <see cref="ContentItem"/>s
/// and speaker notes become the slide notes. Every created <see cref="ContentItem"/>
/// is tagged with <see cref="GetSourceTag(string)"/> so that imported content can be
/// easily found (or removed) later.
/// </summary>
public class SlideDeckImporter
{
    /// <summary>
    /// The prefix of the tag applied to every imported <see cref="ContentItem"/>
    /// </summary>
    public const string SourceTagPrefix = "pptx:";

    const string _markdownContentType = "text/markdown";
    const double _widescreenMinimumRatio = 1.5;
    const AspectRatio _defaultAspectRatio = AspectRatio.Widescreen;

    private static readonly Dictionary<string, string> _imageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/png"] = ".png",
        ["image/jpeg"] = ".jpg",
        ["image/jpg"] = ".jpg",
        ["image/gif"] = ".gif",
        ["image/bmp"] = ".bmp",
        ["image/tiff"] = ".tiff",
        ["image/svg+xml"] = ".svg",
        ["image/webp"] = ".webp",
        ["image/x-emf"] = ".emf",
        ["image/x-wmf"] = ".wmf"
    };

    private readonly ILogger<SlideDeckImporter> _logger;

    public SlideDeckImporter(ILogger<SlideDeckImporter> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    /// <summary>
    /// Builds the tag used to identify content imported from the specified PowerPoint file
    /// </summary>
    /// <param name="sourceName">The name (or path) of the PowerPoint file</param>
    /// <remarks>Commas are replaced since tags are persisted as a comma-delimited list by some repositories</remarks>
    public static string GetSourceTag(string sourceName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceName);
        var fileName = Path.GetFileName(sourceName.Trim())
            .Replace(",", "_", StringComparison.Ordinal)
            .Trim();
        return $"{SourceTagPrefix}{fileName}";
    }

    /// <summary>
    /// Imports the PowerPoint presentation at the specified path
    /// </summary>
    /// <param name="filePath">The path to the .pptx file</param>
    /// <param name="slideNumbers">The 1-based numbers of the slides to import. If null or empty, all slides are imported.</param>
    public SlideDeck Import(string filePath, IEnumerable<int>? slideNumbers = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var fullPath = Path.GetFullPath(filePath);
        if (!File.Exists(fullPath))
        {
            Log.SourceFileNotFound(_logger, fullPath);
            throw new FileNotFoundException("The PowerPoint file could not be found", fullPath);
        }

        using var stream = File.OpenRead(fullPath);
        return this.Import(stream, Path.GetFileName(fullPath), slideNumbers);
    }

    /// <summary>
    /// Imports the PowerPoint presentation contained in the specified stream
    /// </summary>
    /// <param name="stream">A readable, seekable stream containing the .pptx data</param>
    /// <param name="sourceName">The name of the PowerPoint file, used for the deck title fallback and the source tag</param>
    /// <param name="slideNumbers">The 1-based numbers of the slides to import. If null or empty, all slides are imported.</param>
    public SlideDeck Import(Stream stream, string sourceName, IEnumerable<int>? slideNumbers = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceName);

        Log.ImportStarting(_logger, sourceName);

        using var document = this.OpenDocument(stream, sourceName);
        var presentationPart = document.PresentationPart;
        if (presentationPart is null)
        {
            Log.InvalidPresentation(_logger, sourceName, null);
            throw new InvalidDataException($"'{sourceName}' does not contain a presentation");
        }

        var sourceTag = GetSourceTag(sourceName);
        var slideParts = this.GetSlideParts(presentationPart, sourceName);
        var selectedSlideParts = this.SelectSlideParts(slideParts, slideNumbers, sourceName);

        var slides = new List<Slide>();
        foreach (var (number, slidePart) in selectedSlideParts)
            slides.Add(this.BuildSlide(number, slidePart, sourceTag));

        var title = FirstNonBlank(document.PackageProperties.Title, Path.GetFileNameWithoutExtension(sourceName));
        var subTitle = document.PackageProperties.Subject?.Trim() ?? string.Empty;
        var presenter = document.PackageProperties.Creator?.Trim() ?? string.Empty;
        var aspectRatio = this.GetAspectRatio(presentationPart, sourceName);
        Log.DeckProperties(_logger, sourceName, title, subTitle, presenter, aspectRatio, sourceTag);

        var slideDeck = new SlideDeckBuilder()
            .Title(title)
            .SubTitle(subTitle)
            .Presenter(presenter)
            .AspectRatio(aspectRatio)
            .Slides(slides)
            .Build();

        var contentItemCount = slides.Sum(s => s.ContentItems.Count);
        Log.ImportCompleted(_logger, sourceName, slideDeck.Id, slides.Count, contentItemCount);

        return slideDeck;
    }

    private PresentationDocument OpenDocument(Stream stream, string sourceName)
    {
        try
        {
            return PresentationDocument.Open(stream, false);
        }
        catch (Exception ex) when (ex is OpenXmlPackageException or FileFormatException or InvalidDataException or IOException)
        {
            Log.InvalidPresentation(_logger, sourceName, ex);
            throw new InvalidDataException($"'{sourceName}' is not a valid PowerPoint presentation", ex);
        }
    }

    private List<(int Number, SlidePart? Part)> GetSlideParts(PresentationPart presentationPart, string sourceName)
    {
        var result = new List<(int, SlidePart?)>();
        var slideIds = presentationPart.Presentation?.SlideIdList?.Elements<P.SlideId>() ?? [];

        var number = 0;
        foreach (var slideId in slideIds)
        {
            number++;
            var relationshipId = slideId.RelationshipId?.Value;
            SlidePart? slidePart = null;
            if (!string.IsNullOrEmpty(relationshipId)
                && presentationPart.TryGetPartById(relationshipId, out var part))
                slidePart = part as SlidePart;

            if (slidePart is null)
                Log.SlidePartMissing(_logger, number, sourceName);

            result.Add((number, slidePart));
        }

        return result;
    }

    private List<(int Number, SlidePart Part)> SelectSlideParts(List<(int Number, SlidePart? Part)> slideParts, IEnumerable<int>? slideNumbers, string sourceName)
    {
        var requested = slideNumbers?.Distinct().ToHashSet() ?? [];
        foreach (var slideNumber in requested.Where(n => n < 1 || n > slideParts.Count))
        {
            Log.SlideNumberOutOfRange(_logger, slideNumber, slideParts.Count, sourceName);
            throw new ArgumentOutOfRangeException(nameof(slideNumbers), slideNumber,
                $"Slide number {slideNumber} is outside the range 1-{slideParts.Count} for '{sourceName}'");
        }

        var result = slideParts
            .Where(s => requested.Count == 0 || requested.Contains(s.Number))
            .Where(s => s.Part is not null)
            .Select(s => (s.Number, s.Part!))
            .ToList();

        if (requested.Count > 0)
            Log.SlidesSelected(_logger, result.Count, slideParts.Count, sourceName);

        return result;
    }

    private Slide BuildSlide(int slideNumber, SlidePart slidePart, string sourceTag)
    {
        string? title = null;
        var textBlocks = new List<string>();
        var images = new List<ContentItemBuilder>();

        var shapeTree = slidePart.Slide?.CommonSlideData?.ShapeTree;
        var elements = shapeTree?.Descendants() ?? [];
        foreach (var element in elements)
        {
            switch (element)
            {
                case P.Shape shape:
                    var placeholderType = GetPlaceholderType(shape, out var isPlaceholder);
                    var paragraphs = GetParagraphs(shape.TextBody);
                    if (paragraphs.Count == 0 || IsIgnoredPlaceholder(placeholderType))
                        break;

                    if (title is null && placeholderType is P.PlaceholderValues.Title or P.PlaceholderValues.CenteredTitle)
                        title = string.Join(" ", paragraphs.Select(p => p.Text));
                    else
                        textBlocks.Add(AsMarkdown(paragraphs, IsBulletedPlaceholder(isPlaceholder, placeholderType)));
                    break;

                case P.Picture picture:
                    var image = this.BuildImage(picture, slidePart, slideNumber, sourceTag);
                    if (image is not null)
                        images.Add(image);
                    break;

                case P.GraphicFrame graphicFrame:
                    Log.UnsupportedElementSkipped(_logger, graphicFrame.LocalName, slideNumber);
                    break;
            }
        }

        var slideTitle = title ?? string.Create(CultureInfo.InvariantCulture, $"Slide {slideNumber}");
        var layout = textBlocks.Count > 0 && images.Count > 0
            ? Layout.ImageRight
            : Layout.FullPage;

        var slideBuilder = new SlideBuilder()
            .Title(slideTitle)
            .Layout(layout)
            .TransitionIn(Transition.PresentationDefault)
            .TransitionOut(Transition.PresentationDefault)
            .BackgroundTransitionIn(Transition.PresentationDefault)
            .BackgroundTransitionOut(Transition.PresentationDefault)
            .Notes(GetNotes(slidePart));

        for (var i = 0; i < textBlocks.Count; i++)
        {
            var textTitle = textBlocks.Count == 1
                ? $"{slideTitle} - Text"
                : string.Create(CultureInfo.InvariantCulture, $"{slideTitle} - Text {i + 1}");

            slideBuilder.ContentItems(new ContentItemBuilder()
                .ContentType(_markdownContentType)
                .Title(textTitle)
                .Tags([sourceTag])
                .Content(textBlocks[i]));
        }

        foreach (var image in images)
            slideBuilder.ContentItems(image);

        Log.SlideImported(_logger, slideNumber, slideTitle, textBlocks.Count, images.Count, layout);
        return slideBuilder.Build();
    }

    private ContentItemBuilder? BuildImage(P.Picture picture, SlidePart slidePart, int slideNumber, string sourceTag)
    {
        var drawingProperties = picture.NonVisualPictureProperties?.NonVisualDrawingProperties;
        var pictureName = drawingProperties?.Name?.Value ?? string.Empty;

        var embedId = picture.BlipFill?.Blip?.Embed?.Value;
        if (string.IsNullOrEmpty(embedId)
            || !slidePart.TryGetPartById(embedId, out var part)
            || part is not ImagePart imagePart)
        {
            Log.PictureSkipped(_logger, pictureName, slideNumber);
            return null;
        }

        byte[] content;
        using (var source = imagePart.GetStream(FileMode.Open, FileAccess.Read))
        using (var buffer = new MemoryStream())
        {
            source.CopyTo(buffer);
            content = buffer.ToArray();
        }

        var contentType = imagePart.ContentType;
        var fileName = GetImageFileName(imagePart.Uri.OriginalString, contentType);
        Log.ImageImported(_logger, fileName, contentType, content.Length, slideNumber);

        return new ContentItemBuilder()
            .ContentType(contentType)
            .FileName(fileName)
            .Title(FirstNonBlank(drawingProperties?.Description?.Value, pictureName, fileName))
            .Tags([sourceTag])
            .Content(content);
    }

    private AspectRatio GetAspectRatio(PresentationPart presentationPart, string sourceName)
    {
        var slideSize = presentationPart.Presentation?.SlideSize;
        var width = slideSize?.Cx?.Value ?? 0;
        var height = slideSize?.Cy?.Value ?? 0;
        if (width <= 0 || height <= 0)
        {
            Log.AspectRatioDefaulted(_logger, sourceName, _defaultAspectRatio);
            return _defaultAspectRatio;
        }

        return ((double)width / height) >= _widescreenMinimumRatio
            ? AspectRatio.Widescreen
            : AspectRatio.Standard;
    }

    private static string GetImageFileName(string partPath, string contentType)
    {
        // Image parts are not guaranteed to have a meaningful extension (e.g. "image.bin"),
        // but output engines rely on the extension, so derive it from the content type when needed
        var fileName = Path.GetFileName(partPath);
        var extension = Path.GetExtension(fileName);
        return (string.IsNullOrEmpty(extension) || extension.Equals(".bin", StringComparison.OrdinalIgnoreCase))
            && _imageExtensions.TryGetValue(contentType, out var imageExtension)
                ? Path.ChangeExtension(fileName, imageExtension)
                : fileName;
    }

    private static string GetNotes(SlidePart slidePart)
    {
        var shapes = slidePart.NotesSlidePart?.NotesSlide?.CommonSlideData?.ShapeTree?.Descendants<P.Shape>() ?? [];
        var lines = shapes
            .Where(s => GetPlaceholderType(s, out _) == P.PlaceholderValues.Body)
            .SelectMany(s => GetParagraphs(s.TextBody))
            .Select(p => p.Text);
        return string.Join(Environment.NewLine, lines);
    }

    private static P.PlaceholderValues? GetPlaceholderType(P.Shape shape, out bool isPlaceholder)
    {
        var placeholder = shape.NonVisualShapeProperties?.ApplicationNonVisualDrawingProperties?.PlaceholderShape;
        isPlaceholder = placeholder is not null;
        return placeholder?.Type?.Value;
    }

    private static bool IsIgnoredPlaceholder(P.PlaceholderValues? placeholderType)
        => placeholderType is P.PlaceholderValues.DateAndTime
            or P.PlaceholderValues.Footer
            or P.PlaceholderValues.Header
            or P.PlaceholderValues.SlideNumber;

    private static bool IsBulletedPlaceholder(bool isPlaceholder, P.PlaceholderValues? placeholderType)
        => isPlaceholder && placeholderType is null or P.PlaceholderValues.Body or P.PlaceholderValues.Object;

    private static List<(int Level, string Text)> GetParagraphs(OpenXmlElement? textBody)
    {
        var result = new List<(int, string)>();
        if (textBody is null)
            return result;

        foreach (var paragraph in textBody.Elements<D.Paragraph>())
        {
            var text = new StringBuilder();
            foreach (var child in paragraph.ChildElements)
            {
                switch (child)
                {
                    case D.Run run:
                        text.Append(run.Text?.Text);
                        break;
                    case D.Field field:
                        text.Append(field.Text?.Text);
                        break;
                    case D.Break:
                        text.Append(' ');
                        break;
                }
            }

            var value = text.ToString().Trim();
            if (value.Length > 0)
                result.Add((paragraph.ParagraphProperties?.Level?.Value ?? 0, value));
        }

        return result;
    }

    private static string AsMarkdown(List<(int Level, string Text)> paragraphs, bool bulleted)
    {
        var markdown = new StringBuilder();
        foreach (var (level, text) in paragraphs)
        {
            if (bulleted)
                markdown.Append(' ', level * 2).Append("- ").AppendLine(text);
            else
            {
                if (markdown.Length > 0)
                    markdown.AppendLine();
                markdown.AppendLine(text);
            }
        }

        return markdown.ToString().TrimEnd();
    }

    private static string FirstNonBlank(params string?[] values)
        => values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v))?.Trim() ?? string.Empty;
}
