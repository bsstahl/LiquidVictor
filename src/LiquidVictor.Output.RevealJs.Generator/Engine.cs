using System;
using System.Text;
using System.Linq;
using Markdig;
using System.Collections.Generic;
using LiquidVictor.Output.RevealJs.Interfaces;
using LiquidVictor.Output.RevealJs.Extensions;
using LiquidVictor.Entities;
using LiquidVictor.Interfaces;
using LiquidVictor.Extensions;
using System.IO;

namespace LiquidVictor.Output.RevealJs.Generator
{
    public class Engine : IPresentationBuilder
    {
        const string _templateFilename = "index.html";

        string _templatePath;

        public Engine(string templatePath)
        {
            _templatePath = templatePath;
        }

        public void CreatePresentation(string filepath, SlideDeck slideDeck, bool buildTitleSlide)
        {
            var pipeline = new MarkdownPipelineBuilder()
                             .UseAdvancedExtensions()
                             .Build();

            var images = new List<ContentItem>();

            var layoutStrategies = new ILayoutStrategy[Enum.GetValues(typeof(Enumerations.Layout)).Length];
            layoutStrategies[(int)Enumerations.Layout.Title] = new Layout.Title.Engine(pipeline, slideDeck.Transition);
            layoutStrategies[(int)Enumerations.Layout.FullPage] = new Layout.FullPage.Engine(pipeline, slideDeck.Transition);
            layoutStrategies[(int)Enumerations.Layout.FullPageFragments] = new Layout.FullPageFragments.Engine(pipeline, slideDeck.Transition);
            layoutStrategies[(int)Enumerations.Layout.ImageLeft] = new Layout.ImageLeft.Engine(pipeline, slideDeck.Transition);
            layoutStrategies[(int)Enumerations.Layout.ImageRight] = new Layout.ImageRight.Engine(pipeline, slideDeck.Transition);
            layoutStrategies[(int)Enumerations.Layout.ImageWithCaption] = new Layout.ImageWithCaption.Engine(pipeline, slideDeck.Transition);
            layoutStrategies[(int)Enumerations.Layout.MultiColumn] = new Layout.MultiColumn.Engine(pipeline, slideDeck.Transition);

            var slideSections = new StringBuilder();

            // Title slide
            if (buildTitleSlide)
            {
                var titleStrategy = layoutStrategies[(int)Enumerations.Layout.Title];
                var titleSlide = slideDeck.CreateTitleSlide();
                slideSections.AppendLine(titleStrategy.Layout(titleSlide));
            }

            // Content slides
            foreach (var slide in slideDeck.Slides.OrderBy(s => s.Key))
            {
                if (slide.Value.BackgroundContent != null)
                {
                    // Add the ContentItem to the images collection if 
                    // it isn't already there
                    var backgroundContentId = slide.Value.BackgroundContent.Id;
                    if (!images.Any(i => i.Id == slide.Value.BackgroundContent.Id))
                        images.Add(slide.Value.BackgroundContent);
                }

                var strategy = layoutStrategies[(int)slide.Value.Layout];
                if (strategy == null)
                    throw new NotSupportedException($"No layout strategy found for {slide.Value.Layout}");
                // slideSections.AppendLine($"<!-- SlideId={slide.Value.Id.ToString()} -->");
                slideSections.AppendLine(strategy.Layout(slide.Value));
            }

            this.CopyFolder(_templatePath, filepath);
            this.AddImages(slideDeck, images, filepath);

            (int presentationWidth, int presentationHeight) = slideDeck.GetPresentationSize();

            var templatePath = System.IO.Path.Combine(filepath, _templateFilename);
            var indexTemplate = System.IO.File.ReadAllText(templatePath);
            var content = indexTemplate
                .Replace("{SlideSections}", slideSections.ToString())
                .Replace("{Presenter}", slideDeck.Presenter)
                .Replace("{PresentationTitle}", slideDeck.Title)
                .Replace("{ThemeName}", slideDeck.ThemeName.ToLower())
                .Replace("{Transition}", slideDeck.Transition.GetTransitionBaseName())
                .Replace("{Width}", presentationWidth.ToString())
                .Replace("{Height}", presentationHeight.ToString());

            System.IO.File.WriteAllText(templatePath, content);
        }

        private void AddImages(SlideDeck slideDeck, IEnumerable<ContentItem> images, string targetPath)
        {
            string folderPath = System.IO.Path.Combine(targetPath, "img");
            if (!System.IO.Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath);

            foreach (var contentItem in images)
            {
                var fileName = $"{contentItem.Id.ToString()}{Path.GetExtension(contentItem.FileName)}";
                var filePath = System.IO.Path.Combine(folderPath, fileName);
                System.IO.File.WriteAllBytes(filePath, contentItem.Content);
            }

        }

        private void CopyFolder(string sourcePath, string targetPath)
        {
            if (System.IO.Directory.Exists(sourcePath))
            {
                if (!System.IO.Directory.Exists(targetPath))
                    System.IO.Directory.CreateDirectory(targetPath);

                // Copy files
                var sourceFiles = System.IO.Directory.GetFiles(sourcePath);
                foreach (var sourceFile in sourceFiles)
                {
                    var fileName = System.IO.Path.GetFileName(sourceFile);
                    var destFile = System.IO.Path.Combine(targetPath, fileName);

                    System.IO.File.Copy(sourceFile, destFile, true);
                }

                // Copy folders
                var sourceFolders = System.IO.Directory.GetDirectories(sourcePath);
                foreach (var sourceFolder in sourceFolders)
                {
                    string childFolder = sourceFolder.Split(System.IO.Path.DirectorySeparatorChar).Last();
                    string folderTargetPath = System.IO.Path.Combine(targetPath, childFolder);
                    CopyFolder(sourceFolder, folderTargetPath);
                }
            }
        }
    }
}