using Markdig;

namespace LiquidVictor.Output.RevealJs.Test;

/// <summary>
/// Tests that verify LaTeX math expressions are correctly rendered
/// by the Markdig pipeline used in the RevealJS generator.
/// 
/// The RevealJS generator uses Markdig's mathematics extension (included
/// via UseAdvancedExtensions()) to convert LaTeX notation to HTML.
/// The resulting HTML is then rendered by the MathJax plugin bundled
/// with the RevealJS template.
///
/// Inline math:  $...$   → &lt;span class="math"&gt;\(...\)&lt;/span&gt;
/// Display math: $$\n...\n$$ → &lt;div class="math"&gt;\[...\]&lt;/div&gt;
/// </summary>
public class MarkdownPipeline_LaTeX_Should
{
    private readonly MarkdownPipeline _pipeline;

    public MarkdownPipeline_LaTeX_Should()
    {
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void RenderInlineMathWithSpanTag()
    {
        var markdown = "The formula $E = mc^2$ is famous.";
        var html = Markdown.ToHtml(markdown, _pipeline);
        Assert.Contains("<span class=\"math\">", html);
        Assert.Contains(@"\(E = mc^2\)", html);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void RenderBlockMathWithDivTag()
    {
        var markdown = "$$\n\\frac{-b \\pm \\sqrt{b^2 - 4ac}}{2a}\n$$";
        var html = Markdown.ToHtml(markdown, _pipeline);
        Assert.Contains("<div class=\"math\">", html);
        Assert.Contains(@"\[", html);
        Assert.Contains(@"\]", html);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void RenderGreekLettersInInlineMath()
    {
        var markdown = "The angle $\\theta$ and constant $\\pi$ appear often in trigonometry.";
        var html = Markdown.ToHtml(markdown, _pipeline);
        Assert.Contains("<span class=\"math\">", html);
        Assert.Contains(@"\(\theta\)", html);
        Assert.Contains(@"\(\pi\)", html);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void RenderSummationInDisplayMath()
    {
        var markdown = "$$\n\\sum_{i=1}^{n} i = \\frac{n(n+1)}{2}\n$$";
        var html = Markdown.ToHtml(markdown, _pipeline);
        Assert.Contains("<div class=\"math\">", html);
        Assert.Contains(@"\sum_{i=1}^{n}", html);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void RenderMatrixInDisplayMath()
    {
        var markdown = "$$\n\\begin{pmatrix} a & b \\\\ c & d \\end{pmatrix}\n$$";
        var html = Markdown.ToHtml(markdown, _pipeline);
        Assert.Contains("<div class=\"math\">", html);
        Assert.Contains(@"\begin{pmatrix}", html);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void PreserveRegularMarkdownAlongsideMath()
    {
        var markdown = "## Heading\n\nSome **bold** text and $x^2$ inline math.";
        var html = Markdown.ToHtml(markdown, _pipeline);
        Assert.Contains("<h2", html);
        Assert.Contains("<strong>bold</strong>", html);
        Assert.Contains("<span class=\"math\">", html);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void NotRenderMathWhenNoDelimitersPresent()
    {
        var markdown = "This is plain text with no math.";
        var html = Markdown.ToHtml(markdown, _pipeline);
        Assert.DoesNotContain("<span class=\"math\">", html);
        Assert.DoesNotContain("<div class=\"math\">", html);
    }
}
