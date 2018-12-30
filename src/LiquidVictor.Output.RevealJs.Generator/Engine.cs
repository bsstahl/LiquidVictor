using LiquidVictor.Entities;
using LiquidVictor.Interfaces;
using System;
using System.Text;
using LiquidVictor.Extensions;
using System.Linq;
using Markdig;
using System.Collections.Generic;
using LiquidVictor.Output.RevealJs.Interfaces;
using LiquidVictor.Output.RevealJs.Extensions;

namespace LiquidVictor.Output.RevealJs.Generator
{
    public class Engine : IPresentationBuilder
    {
        const string _templateFolder = @"..\..\..\..\..\Templates\RevealJs";
        const string _templateFilename = "index.html";

        public void CreatePresentation(string filepath, SlideDeck slideDeck)
        {
            var pipeline = new MarkdownPipelineBuilder()
                             .UseAdvancedExtensions()
                             .Build();

            var layoutStrategies = new ILayoutStrategy[Enum.GetValues(typeof(Enumerations.Layout)).Length];
            layoutStrategies[(int)Enumerations.Layout.Title] = new Layout.Title.Engine(pipeline);
            layoutStrategies[(int)Enumerations.Layout.FullPage] = new Layout.FullPage.Engine(pipeline);
            layoutStrategies[(int)Enumerations.Layout.FullPageFragments] = new Layout.FullPageFragments.Engine(pipeline);
            layoutStrategies[(int)Enumerations.Layout.ImageLeft] = new Layout.ImageLeft.Engine(pipeline);
            layoutStrategies[(int)Enumerations.Layout.ImageRight] = new Layout.ImageRight.Engine(pipeline);
            layoutStrategies[(int)Enumerations.Layout.ImageWithCaption] = new Layout.ImageWithCaption.Engine(pipeline);
            layoutStrategies[(int)Enumerations.Layout.MultiColumn] = new Layout.MultiColumn.Engine(pipeline);

            var slideSections = new StringBuilder();

            // Title slide
            var titleStrategy = layoutStrategies[(int)Enumerations.Layout.Title];
            var titleSlide = slideDeck.CreateTitleSlide();
            slideSections.AppendLine(titleStrategy.Layout(titleSlide));

            // Content slides
            foreach (var slide in slideDeck.Slides.OrderBy(s => s.Key))
            {
                var strategy = layoutStrategies[(int)slide.Value.Layout];
                if (strategy == null)
                    throw new NotSupportedException($"No layout strategy found for {slide.Value.Layout}");
                slideSections.AppendLine(strategy.Layout(slide.Value));
            }

            CopyFolder(_templateFolder, filepath);

            (int presentationWidth, int presentationHeight) = slideDeck.GetPresentationSize();

            var templatePath = System.IO.Path.Combine(filepath, _templateFilename);
            var indexTemplate = System.IO.File.ReadAllText(templatePath);
            var content = indexTemplate
                .Replace("{SlideSections}", slideSections.ToString())
                .Replace("{PresentationTitle}", slideDeck.Title)
                .Replace("{ThemeName}", slideDeck.ThemeName.ToLower())
                .Replace("{Width}", presentationWidth.ToString())
                .Replace("{Height}", presentationHeight.ToString());

            System.IO.File.WriteAllText(templatePath, content);
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