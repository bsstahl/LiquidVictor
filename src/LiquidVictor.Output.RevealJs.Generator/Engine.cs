using System;
using System.Text;
using System.Linq;
using Markdig;
using System.Collections.Generic;
using LiquidVictor.Output.RevealJs.Interfaces;
using LiquidVictor.Output.RevealJs.Extensions;
using LiquidVictor.Entities;
using LiquidVictor.Interfaces;
using System.IO;
using LiquidVictor.Output.RevealJs.Entities;
using System.Globalization;
using LiquidVictor.Output.RevealJs.Generator.Extensions;

namespace LiquidVictor.Output.RevealJs.Generator;

public class Engine : IPresentationBuilder
{
    const string _templateFilename = "index.html";

    readonly string _templatePath;
    readonly BuilderOptions _builderOptions;

    public Engine(string templatePath, BuilderOptions builderOptions)
    {
        _templatePath = templatePath;
        _builderOptions = builderOptions;
    }

    public void CompilePresentation(SlideDeck slideDeck)
    {
        ArgumentNullException.ThrowIfNull(slideDeck);
        _ = this.BuildContent(slideDeck, _builderOptions);
    }

    public void CreatePresentation(string filepath, SlideDeck slideDeck)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(filepath);
        ArgumentNullException.ThrowIfNull(slideDeck);

        var (images, content) = this.BuildContent(slideDeck, _builderOptions);
        this.WriteContent(filepath, images, content);
    }

    private (IEnumerable<ContentItem>, string) BuildContent(SlideDeck slideDeck, BuilderOptions builderOptions)
    {
        // UseAdvancedExtensions includes UseMathematics(), which enables LaTeX rendering
        // via $...$ (inline) and $$\n...\n$$ (display) delimiters.
        // The resulting HTML uses \(...\) and \[...\] notation, which is processed
        // by the MathJax plugin bundled in the RevealJS template.
        // See docs/latex-support.md for usage examples and documentation.
        var pipeline = new MarkdownPipelineBuilder()
                         .UseAdvancedExtensions()
                         .Build();

        var images = new List<ContentItem>();
        var layoutStrategies = GetLayoutStrategies(pipeline, builderOptions, slideDeck);
        var slideSections = new StringBuilder();

        int slideIndex = 0;

        if (builderOptions.BuildTitleSlide)
        {
            var titleSlide = slideDeck.CreateTitleSlide();
            var titleStrategy = layoutStrategies[(int)Enumerations.Layout.Title];
            slideSections.AppendLine(titleStrategy.Layout(titleSlide, slideIndex));
            slideIndex++;
        }

        // Content slides
        foreach (var slide in slideDeck.Slides.OrderBy(s => s.Key))
        {
            images.AddFromSlide(slide.Value);
            slideSections.AppendLine(slide.Value.GetLayout(slideIndex, layoutStrategies));
            slideIndex++;
        }

        (int presentationWidth, int presentationHeight) = slideDeck.GetPresentationSize();

        var templateFilePath = Path.Combine(_templatePath, _templateFilename);
        var indexTemplate = File.ReadAllText(templateFilePath);

        var content = indexTemplate
            .Replace("{SlideSections}", slideSections.ToString(), StringComparison.Ordinal)
            .Replace("{Presenter}", slideDeck.Presenter, StringComparison.Ordinal)
            .Replace("{PresentationTitle}", slideDeck.Title, StringComparison.Ordinal)
            .Replace("{ThemeName}", slideDeck.ThemeName.ToLower(CultureInfo.CurrentCulture), StringComparison.Ordinal)
            .Replace("{Transition}", slideDeck.Transition.GetTransitionBaseName(), StringComparison.Ordinal)
            .Replace("{BackgroundTransition}", slideDeck.BackgroundTransition.GetTransitionBaseName(), StringComparison.Ordinal)
            .Replace("{Width}", presentationWidth.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)
            .Replace("{Height}", presentationHeight.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);

        return (images, content);
    }

    private void WriteContent(string filepath, IEnumerable<ContentItem> images, string content)
    {
        var outputFilePath = Path.Combine(filepath, _templateFilename);
        _templatePath.CopyFolder(filepath);
        images.AddImages(filepath);
        File.WriteAllText(outputFilePath, content);
    }

    private static ILayoutStrategy[] GetLayoutStrategies(MarkdownPipeline pipeline, BuilderOptions builderOptions, SlideDeck slideDeck)
    {
        var layoutStrategies = new ILayoutStrategy[Enum.GetValues<Enumerations.Layout>().Length];
        layoutStrategies[(int)Enumerations.Layout.Title] = new Layout.Title.Engine(pipeline, slideDeck.Transition, slideDeck.BackgroundTransition, builderOptions);
        layoutStrategies[(int)Enumerations.Layout.FullPage] = new Layout.FullPage.Engine(pipeline, slideDeck.Transition, slideDeck.BackgroundTransition, builderOptions);
        layoutStrategies[(int)Enumerations.Layout.FullPageFragments] = new Layout.FullPageFragments.Engine(pipeline, slideDeck.Transition, slideDeck.BackgroundTransition, builderOptions);
        layoutStrategies[(int)Enumerations.Layout.ImageLeft] = new Layout.ImageLeft.Engine(pipeline, slideDeck.Transition, slideDeck.BackgroundTransition, builderOptions);
        layoutStrategies[(int)Enumerations.Layout.ImageRight] = new Layout.ImageRight.Engine(pipeline, slideDeck.Transition, slideDeck.BackgroundTransition, builderOptions);
        layoutStrategies[(int)Enumerations.Layout.ImageWithCaption] = new Layout.ImageWithCaption.Engine(pipeline, slideDeck.Transition, slideDeck.BackgroundTransition, builderOptions);
        layoutStrategies[(int)Enumerations.Layout.MultiColumn] = new Layout.MultiColumn.Engine(pipeline, slideDeck.Transition, slideDeck.BackgroundTransition, builderOptions);
        layoutStrategies[(int)Enumerations.Layout.MultiSlide] = new Layout.MultiSlide.Engine(pipeline, slideDeck.Transition, slideDeck.BackgroundTransition, builderOptions);
        return layoutStrategies;
    }
}