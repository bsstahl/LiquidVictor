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

namespace LiquidVictor.Output.RevealJs.Generator
{
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
            _ = BuildContent(slideDeck, _builderOptions);
        }

        public void CreatePresentation(string filepath, SlideDeck slideDeck)
        {
            var (images, content) = BuildContent(slideDeck, _builderOptions);
            WriteContent(filepath, images, content);
        }

        private (IEnumerable<ContentItem>, string) BuildContent(SlideDeck slideDeck, BuilderOptions builderOptions)
        {
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
                .Replace("{SlideSections}", slideSections.ToString())
                .Replace("{Presenter}", slideDeck.Presenter)
                .Replace("{PresentationTitle}", slideDeck.Title)
                .Replace("{ThemeName}", slideDeck.ThemeName.ToLower())
                .Replace("{Transition}", slideDeck.Transition.GetTransitionBaseName())
                .Replace("{Width}", presentationWidth.ToString())
                .Replace("{Height}", presentationHeight.ToString());

            return (images, content);
        }

        private void WriteContent(string filepath, IEnumerable<ContentItem> images, string content)
        {
            var outputFilePath = Path.Combine(filepath, _templateFilename);
            this.CopyFolder(_templatePath, filepath);
            this.AddImages(images, filepath);
            File.WriteAllText(outputFilePath, content);
        }

        private static ILayoutStrategy[] GetLayoutStrategies(MarkdownPipeline pipeline, BuilderOptions builderOptions, SlideDeck slideDeck)
        {
            var layoutStrategies = new ILayoutStrategy[Enum.GetValues(typeof(Enumerations.Layout)).Length];
            layoutStrategies[(int)Enumerations.Layout.Title] = new Layout.Title.Engine(pipeline, slideDeck.Transition, builderOptions);
            layoutStrategies[(int)Enumerations.Layout.FullPage] = new Layout.FullPage.Engine(pipeline, slideDeck.Transition, builderOptions);
            layoutStrategies[(int)Enumerations.Layout.FullPageFragments] = new Layout.FullPageFragments.Engine(pipeline, slideDeck.Transition, builderOptions);
            layoutStrategies[(int)Enumerations.Layout.ImageLeft] = new Layout.ImageLeft.Engine(pipeline, slideDeck.Transition, builderOptions);
            layoutStrategies[(int)Enumerations.Layout.ImageRight] = new Layout.ImageRight.Engine(pipeline, slideDeck.Transition, builderOptions);
            layoutStrategies[(int)Enumerations.Layout.ImageWithCaption] = new Layout.ImageWithCaption.Engine(pipeline, slideDeck.Transition, builderOptions);
            layoutStrategies[(int)Enumerations.Layout.MultiColumn] = new Layout.MultiColumn.Engine(pipeline, slideDeck.Transition, builderOptions);
            layoutStrategies[(int)Enumerations.Layout.MultiSlide] = new Layout.MultiSlide.Engine(pipeline, slideDeck.Transition, builderOptions);
            return layoutStrategies;
        }

        private void AddImages(IEnumerable<ContentItem> images, string targetPath)
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
                    this.CopyFolder(sourceFolder, folderTargetPath);
                }
            }
        }
    }
}