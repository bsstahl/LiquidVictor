using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using LiquidVictor.Entities;
using LiquidVictor.Interfaces;
using LiquidVictor.Output.Jupyter.Entities;
using LiquidVictor.Output.Jupyter.Extensions;

namespace LiquidVictor.Output.Jupyter.Generator;

public class Engine(BuilderOptions builderOptions) : IPresentationBuilder
{
    private const string DefaultNotebookFilename = "presentation.ipynb";
    private readonly BuilderOptions _builderOptions = builderOptions ?? throw new ArgumentNullException(nameof(builderOptions));

    public void CompilePresentation(SlideDeck slideDeck)
    {
        ArgumentNullException.ThrowIfNull(slideDeck);
        _ = BuildNotebookJson(slideDeck);
    }

    public void CreatePresentation(string filepath, SlideDeck slideDeck)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(filepath);
        ArgumentNullException.ThrowIfNull(slideDeck);

        var outputFilePath = ResolveOutputFilePath(filepath);
        var outputFolder = Path.GetDirectoryName(outputFilePath) ?? throw new InvalidOperationException("Could not determine output directory");
        Directory.CreateDirectory(outputFolder);

        var notebook = BuildNotebookJson(slideDeck);
        File.WriteAllText(outputFilePath, notebook);
    }

    private string BuildNotebookJson(SlideDeck slideDeck)
    {
        var cells = new List<NotebookCell>();
        if (_builderOptions.BuildTitleSlide)
            cells.Add(ToCell(slideDeck.CreateTitleSlide()));

        foreach (var slide in slideDeck.Slides.OrderBy(s => s.Key).Select(s => s.Value))
        {
            cells.Add(ToCell(slide));
            if (!string.IsNullOrWhiteSpace(slide.Notes))
                cells.Add(ToCell($"### Presenter Notes{Environment.NewLine}{Environment.NewLine}{slide.Notes}", "notes"));
        }

        var notebook = new NotebookDocument
        {
            Cells = cells,
            Metadata = new NotebookMetadata
            {
                LiveReveal = new LiveRevealMetadata
                {
                    Theme = string.IsNullOrWhiteSpace(slideDeck.ThemeName) ? "black" : slideDeck.ThemeName,
                    Transition = slideDeck.Transition.ToString()
                }
            }
        };

        return JsonSerializer.Serialize(notebook, SerializerOptions);
    }

    private static NotebookCell ToCell(Slide slide)
    {
        ArgumentNullException.ThrowIfNull(slide);

        var markdown = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(slide.Title))
        {
            markdown.Append("# ");
            markdown.AppendLine(slide.Title);
            markdown.AppendLine();
        }

        if (slide.BackgroundContent is not null)
            markdown.AppendLine(ToMarkdown(slide.BackgroundContent));

        foreach (var contentItem in slide.ContentItems.OrderBy(c => c.Key).Select(c => c.Value))
            markdown.AppendLine(ToMarkdown(contentItem));

        return ToCell(markdown.ToString(), "slide");
    }

    private static NotebookCell ToCell(string markdown, string slideType)
    {
        return new NotebookCell
        {
            Metadata = new CellMetadata
            {
                Slideshow = new SlideshowMetadata
                {
                    SlideType = slideType
                }
            },
            Source = ToSourceLines(markdown)
        };
    }

    private static List<string> ToSourceLines(string content)
    {
        var normalized = content.Replace("\r\n", "\n", StringComparison.Ordinal);
        if (normalized.Length == 0)
            return [];

        var lines = normalized.Split('\n');
        var source = new List<string>(lines.Length);
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            source.Add(i < lines.Length - 1 ? $"{line}\n" : line);
        }

        return source;
    }

    private static string ToMarkdown(ContentItem contentItem)
    {
        ArgumentNullException.ThrowIfNull(contentItem);

        if (contentItem.ContentType.StartsWith("text", StringComparison.OrdinalIgnoreCase))
            return Encoding.UTF8.GetString(contentItem.Content);

        if (contentItem.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
        {
            var altText = string.IsNullOrWhiteSpace(contentItem.Title) ? contentItem.FileName : contentItem.Title;
            return $"![{altText}]({contentItem.AsImageSource()})";
        }

        throw new NotSupportedException("Only Text and Image content is currently supported");
    }

    private static string ResolveOutputFilePath(string path)
    {
        if (path.EndsWith(".ipynb", StringComparison.OrdinalIgnoreCase))
            return path;

        return Path.Combine(path, DefaultNotebookFilename);
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private sealed class NotebookDocument
    {
        [JsonPropertyName("cells")]
        public List<NotebookCell> Cells { get; set; } = [];
        [JsonPropertyName("metadata")]
        public NotebookMetadata Metadata { get; set; } = new();
        [JsonPropertyName("nbformat")]
        public int Nbformat { get; set; } = 4;
        [JsonPropertyName("nbformat_minor")]
        public int NbformatMinor { get; set; } = 5;
    }

    private sealed class NotebookCell
    {
        [JsonPropertyName("cell_type")]
        public string CellType { get; set; } = "markdown";
        [JsonPropertyName("metadata")]
        public CellMetadata Metadata { get; set; } = new();
        [JsonPropertyName("source")]
        public List<string> Source { get; set; } = [];
    }

    private sealed class CellMetadata
    {
        [JsonPropertyName("slideshow")]
        public SlideshowMetadata Slideshow { get; set; } = new();
    }

    private sealed class SlideshowMetadata
    {
        [JsonPropertyName("slide_type")]
        public string SlideType { get; set; } = "slide";
    }

    private sealed class NotebookMetadata
    {
        [JsonPropertyName("kernelspec")]
        public KernelSpecMetadata KernelSpec { get; set; } = new();
        [JsonPropertyName("language_info")]
        public LanguageInfoMetadata LanguageInfo { get; set; } = new();
        [JsonPropertyName("livereveal")]
        public LiveRevealMetadata LiveReveal { get; set; } = new();
    }

    private sealed class KernelSpecMetadata
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = "Python 3";
        [JsonPropertyName("language")]
        public string Language { get; set; } = "python";
        [JsonPropertyName("name")]
        public string Name { get; set; } = "python3";
    }

    private sealed class LanguageInfoMetadata
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "python";
    }

    private sealed class LiveRevealMetadata
    {
        [JsonPropertyName("autolaunch")]
        public bool AutoLaunch { get; set; }
        [JsonPropertyName("theme")]
        public string Theme { get; set; } = "black";
        [JsonPropertyName("transition")]
        public string Transition { get; set; } = "slide";
    }
}
