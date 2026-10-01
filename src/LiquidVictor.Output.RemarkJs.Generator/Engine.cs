using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using LiquidVictor.Entities;
using LiquidVictor.Interfaces;
using Microsoft.Extensions.Logging;

namespace LiquidVictor.Output.RemarkJs.Generator;

public class Engine(ILogger<Engine> logger, bool buildTitleSlide) : IPresentationBuilder
{
    const string _indexFilename = "index.html";
    const string _imageDirectory = "img";

    private readonly ILogger<Engine>     _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly bool _buildTitleSlide = buildTitleSlide;

    public void CompilePresentation(SlideDeck slideDeck)
    {
        ArgumentNullException.ThrowIfNull(slideDeck);

        Log.CompilationStarted(_logger);
        Log.CompilationDetails(_logger, slideDeck.Title);
        try
        {
            _ = BuildMarkdown(slideDeck);
            Log.CompilationCompleted(_logger);
        }
        catch (Exception exception)
        {
            Log.CompilationFailed(_logger, exception);
            throw;
        }
    }

    public void CreatePresentation(string filepath, SlideDeck slideDeck)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filepath);
        ArgumentNullException.ThrowIfNull(slideDeck);

        Log.GenerationStarted(_logger);
        Log.GenerationDetails(_logger, slideDeck.Title, filepath);
        try
        {
            var (images, markdown) = BuildMarkdown(slideDeck);

            Directory.CreateDirectory(filepath);
            foreach (var image in images)
            {
                var imagePath = Path.Combine(filepath, _imageDirectory, GetImageFileName(image));
                Directory.CreateDirectory(Path.GetDirectoryName(imagePath)!);
                File.WriteAllBytes(imagePath, image.Content);
            }

            var html = CreateHtml(markdown);
            File.WriteAllText(Path.Combine(filepath, _indexFilename), html);
            Log.GenerationCompleted(_logger, slideDeck.Slides.Count + (_buildTitleSlide ? 1 : 0), images.Count);
        }
        catch (Exception exception)
        {
            Log.GenerationFailed(_logger, exception);
            throw;
        }
    }

    private (IReadOnlyCollection<ContentItem> Images, string Markdown) BuildMarkdown(SlideDeck slideDeck)
    {
        var images = new Dictionary<Guid, ContentItem>();
        var markdown = new StringBuilder();

        if (_buildTitleSlide)
        {
            AddSlideSeparator(markdown);
            markdown.AppendLine("class: center, middle");
            AddBackground(slideDeck.BackgroundContent, markdown, images);
            markdown.AppendLine();
            markdown.AppendLine(CultureInfo.InvariantCulture, $"# {slideDeck.Title}");
            if (!string.IsNullOrWhiteSpace(slideDeck.SubTitle))
                markdown.AppendLine(CultureInfo.InvariantCulture, $"## {slideDeck.SubTitle}");
            if (!string.IsNullOrWhiteSpace(slideDeck.Presenter))
                markdown.AppendLine(CultureInfo.InvariantCulture, $"### {slideDeck.Presenter}");
        }

        foreach (var slide in slideDeck.Slides.OrderBy(s => s.Key).Select(s => s.Value))
        {
            AddSlideSeparator(markdown);
            AddBackground(slide.BackgroundContent ?? slideDeck.BackgroundContent, markdown, images);

            if (!string.IsNullOrWhiteSpace(slide.Title))
                markdown.AppendLine(CultureInfo.InvariantCulture, $"# {slide.Title}");

            foreach (var contentItem in slide.ContentItems.OrderBy(c => c.Key).Select(c => c.Value))
            {
                if (IsImage(contentItem))
                {
                    images.TryAdd(contentItem.Id, contentItem);
                    markdown.AppendLine(CultureInfo.InvariantCulture,
                        $"![{EscapeMarkdownImageAlt(contentItem.FileName)}]({_imageDirectory}/{GetImageFileName(contentItem)})");
                }
                else if (IsText(contentItem))
                {
                    markdown.AppendLine(Encoding.UTF8.GetString(contentItem.Content));
                }
                else
                {
                    throw new NotSupportedException($"Content type '{contentItem.ContentType}' is not supported by Remark.js output");
                }
            }

            if (!string.IsNullOrWhiteSpace(slide.Notes))
            {
                markdown.AppendLine("???");
                markdown.AppendLine(slide.Notes);
            }
        }

        return (images.Values.ToArray(), markdown.ToString().TrimStart());
    }

    private static void AddBackground(ContentItem? background, StringBuilder markdown, IDictionary<Guid, ContentItem> images)
    {
        if (background is null || !IsImage(background))
            return;

        images.TryAdd(background.Id, background);
        markdown.AppendLine(CultureInfo.InvariantCulture,
            $"background-image: url({_imageDirectory}/{GetImageFileName(background)})");
        markdown.AppendLine("background-size: cover");
        markdown.AppendLine();
    }

    private static void AddSlideSeparator(StringBuilder markdown)
    {
        if (markdown.Length > 0)
            markdown.AppendLine().AppendLine("---").AppendLine();
    }

    private static bool IsImage(ContentItem item) =>
        item.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase);

    private static bool IsText(ContentItem item) =>
        item.ContentType.StartsWith("text", StringComparison.OrdinalIgnoreCase);

    private static string GetImageFileName(ContentItem image)
    {
        var extension = Path.GetExtension(image.FileName);
        if (extension.Length is < 2 or > 11 || extension[1..].Any(c => !char.IsAsciiLetterOrDigit(c)))
            extension = string.Empty;

        return $"{image.Id:D}{extension}";
    }

    private static string EscapeMarkdownImageAlt(string fileName) =>
        fileName.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("]", "\\]", StringComparison.Ordinal);

    private static string CreateHtml(string markdown)
    {
        var scriptNonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

        return $$"""
        <!doctype html>
        <html lang="en">
        <head>
          <meta charset="utf-8">
          <meta name="viewport" content="width=device-width, initial-scale=1">
          <meta http-equiv="Content-Security-Policy" content="default-src 'none'; script-src 'nonce-{{scriptNonce}}' https://remarkjs.com; style-src 'unsafe-inline'; img-src 'self' file: http: https: data:; font-src http: https: data:; base-uri 'none'">
          <title>Remark.js presentation</title>
        </head>
        <body>
          <textarea id="source" style="display:none;">{{WebUtility.HtmlEncode(markdown)}}</textarea>
          <script src="https://remarkjs.com/downloads/remark-latest.min.js"></script>
          <script nonce="{{scriptNonce}}">remark.create();</script>
        </body>
        </html>
        """;
    }
}
