using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using D = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

namespace LiquidVictor.Input.Powerpoint.Test.Builders;

/// <summary>
/// Builds minimal PowerPoint (.pptx) documents for use in tests
/// </summary>
public sealed class PowerpointFileBuilder
{
    private readonly List<PowerpointSlideBuilder?> _slides = [];
    private string? _title;
    private string? _subject;
    private string? _creator;
    private (int Width, int Height)? _slideSize;

    public PowerpointFileBuilder Title(string value)
    {
        _title = value;
        return this;
    }

    public PowerpointFileBuilder Subject(string value)
    {
        _subject = value;
        return this;
    }

    public PowerpointFileBuilder Creator(string value)
    {
        _creator = value;
        return this;
    }

    public PowerpointFileBuilder SlideSize(int width, int height)
    {
        _slideSize = (width, height);
        return this;
    }

    public PowerpointFileBuilder AddSlide(Action<PowerpointSlideBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var slide = new PowerpointSlideBuilder();
        configure(slide);
        _slides.Add(slide);
        return this;
    }

    /// <summary>
    /// Adds a slide reference that does not resolve to a slide part
    /// </summary>
    public PowerpointFileBuilder AddUnresolvableSlide()
    {
        _slides.Add(null);
        return this;
    }

    public MemoryStream Build()
    {
        var stream = new MemoryStream();
        using (var document = PresentationDocument.Create(stream, PresentationDocumentType.Presentation))
        {
            var presentationPart = document.AddPresentationPart();
            presentationPart.Presentation = new P.Presentation();

            var slideIdList = new P.SlideIdList();
            uint slideId = 256;
            foreach (var slide in _slides)
            {
                var relationshipId = slide is null
                    ? "rIdMissing"
                    : presentationPart.GetIdOfPart(slide.Build(presentationPart));
                slideIdList.Append(new P.SlideId() { Id = slideId++, RelationshipId = relationshipId });
            }

            presentationPart.Presentation.Append(slideIdList);
            if (_slideSize.HasValue)
                presentationPart.Presentation.Append(new P.SlideSize() { Cx = _slideSize.Value.Width, Cy = _slideSize.Value.Height });

            if (_title is not null)
                document.PackageProperties.Title = _title;
            if (_subject is not null)
                document.PackageProperties.Subject = _subject;
            if (_creator is not null)
                document.PackageProperties.Creator = _creator;
        }

        stream.Position = 0;
        return stream;
    }

    public string BuildFile(string filePath)
    {
        using var stream = this.Build();
        File.WriteAllBytes(filePath, stream.ToArray());
        return filePath;
    }
}

public sealed class PowerpointSlideBuilder
{
    private readonly List<Func<SlidePart, uint, OpenXmlElement>> _elements = [];
    private string? _notes;

    public PowerpointSlideBuilder Title(string text) => this.Placeholder(P.PlaceholderValues.Title, "Title", text);

    public PowerpointSlideBuilder CenteredTitle(string text) => this.Placeholder(P.PlaceholderValues.CenteredTitle, "Title", text);

    public PowerpointSlideBuilder SubTitle(string text) => this.Placeholder(P.PlaceholderValues.SubTitle, "Subtitle", text);

    public PowerpointSlideBuilder Footer(string text) => this.Placeholder(P.PlaceholderValues.Footer, "Footer", text);

    /// <summary>
    /// Adds a content placeholder (a placeholder with no explicit type) whose paragraphs are rendered as bullets
    /// </summary>
    public PowerpointSlideBuilder Body(params (int Level, string Text)[] paragraphs)
    {
        _elements.Add((_, id) => CreateShape(id, "Content Placeholder", isPlaceholder: true, null, paragraphs));
        return this;
    }

    public PowerpointSlideBuilder Body(params string[] paragraphs)
        => this.Body(paragraphs.Select(p => (0, p)).ToArray());

    public PowerpointSlideBuilder TextBox(params string[] paragraphs)
    {
        _elements.Add((_, id) => CreateShape(id, "TextBox", isPlaceholder: false, null, paragraphs.Select(p => (0, p))));
        return this;
    }

    public PowerpointSlideBuilder GroupedTextBox(params string[] paragraphs)
    {
        _elements.Add((_, id) => new P.GroupShape(
            new P.NonVisualGroupShapeProperties(
                new P.NonVisualDrawingProperties() { Id = id, Name = "Group" },
                new P.NonVisualGroupShapeDrawingProperties(),
                new P.ApplicationNonVisualDrawingProperties()),
            new P.GroupShapeProperties(),
            CreateShape(id + 1000, "Grouped TextBox", isPlaceholder: false, null, paragraphs.Select(p => (0, p)))));
        return this;
    }

    public PowerpointSlideBuilder Picture(byte[] content, string contentType = "image/png", string name = "Picture", string? description = null)
    {
        _elements.Add((slidePart, id) =>
        {
            var imagePart = slidePart.AddImagePart(contentType);
            using (var data = new MemoryStream(content))
                imagePart.FeedData(data);

            return CreatePicture(id, name, description, new D.Blip() { Embed = slidePart.GetIdOfPart(imagePart) });
        });
        return this;
    }

    public PowerpointSlideBuilder LinkedPicture(string name = "Linked Picture")
    {
        _elements.Add((_, id) => CreatePicture(id, name, null, new D.Blip()));
        return this;
    }

    public PowerpointSlideBuilder Table(string name = "Table")
    {
        _elements.Add((_, id) => new P.GraphicFrame(
            new P.NonVisualGraphicFrameProperties(
                new P.NonVisualDrawingProperties() { Id = id, Name = name },
                new P.NonVisualGraphicFrameDrawingProperties(),
                new P.ApplicationNonVisualDrawingProperties()),
            new P.Transform(),
            new D.Graphic()));
        return this;
    }

    public PowerpointSlideBuilder Notes(string text)
    {
        _notes = text;
        return this;
    }

    internal SlidePart Build(PresentationPart presentationPart)
    {
        var slidePart = presentationPart.AddNewPart<SlidePart>();
        var shapeTree = CreateShapeTree();

        uint id = 2;
        foreach (var element in _elements)
            shapeTree.Append(element(slidePart, id++));

        slidePart.Slide = new P.Slide(new P.CommonSlideData(shapeTree));

        if (_notes is not null)
        {
            var notesPart = slidePart.AddNewPart<NotesSlidePart>();
            var notesShapeTree = CreateShapeTree();
            notesShapeTree.Append(CreateShape(2, "Slide Image Placeholder", isPlaceholder: true, P.PlaceholderValues.SlideImage, []));
            notesShapeTree.Append(CreateShape(3, "Notes Placeholder", isPlaceholder: true, P.PlaceholderValues.Body,
                _notes.Split('\n').Select(line => (0, line))));
            notesPart.NotesSlide = new P.NotesSlide(new P.CommonSlideData(notesShapeTree));
        }

        return slidePart;
    }

    private PowerpointSlideBuilder Placeholder(P.PlaceholderValues type, string name, string text)
    {
        _elements.Add((_, id) => CreateShape(id, name, isPlaceholder: true, type, [(0, text)]));
        return this;
    }

    private static P.ShapeTree CreateShapeTree()
    {
        return new P.ShapeTree(
            new P.NonVisualGroupShapeProperties(
                new P.NonVisualDrawingProperties() { Id = 1, Name = string.Empty },
                new P.NonVisualGroupShapeDrawingProperties(),
                new P.ApplicationNonVisualDrawingProperties()),
            new P.GroupShapeProperties());
    }

    private static P.Shape CreateShape(uint id, string name, bool isPlaceholder, P.PlaceholderValues? type, IEnumerable<(int Level, string Text)> paragraphs)
    {
        var applicationProperties = new P.ApplicationNonVisualDrawingProperties();
        if (isPlaceholder)
        {
            var placeholder = new P.PlaceholderShape();
            if (type.HasValue)
                placeholder.Type = type.Value;
            applicationProperties.Append(placeholder);
        }

        var textBody = new P.TextBody(new D.BodyProperties(), new D.ListStyle());
        foreach (var (level, text) in paragraphs)
            textBody.Append(new D.Paragraph(
                new D.ParagraphProperties() { Level = level },
                new D.Run(new D.Text(text))));

        return new P.Shape(
            new P.NonVisualShapeProperties(
                new P.NonVisualDrawingProperties() { Id = id, Name = name },
                new P.NonVisualShapeDrawingProperties(),
                applicationProperties),
            new P.ShapeProperties(),
            textBody);
    }

    private static P.Picture CreatePicture(uint id, string name, string? description, D.Blip blip)
    {
        var drawingProperties = new P.NonVisualDrawingProperties() { Id = id, Name = name };
        if (description is not null)
            drawingProperties.Description = description;

        return new P.Picture(
            new P.NonVisualPictureProperties(
                drawingProperties,
                new P.NonVisualPictureDrawingProperties(),
                new P.ApplicationNonVisualDrawingProperties()),
            new P.BlipFill(blip, new D.Stretch(new D.FillRectangle())),
            new P.ShapeProperties());
    }
}
