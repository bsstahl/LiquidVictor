using LiquidVictor.Entities;
using LiquidVictor.Interfaces;
using System;
using System.Text;
using LiquidVictor.Extensions;
using System.Linq;

namespace LiquidVictor.Output.RevealJs
{
    public class Engine : IPresentationBuilder
    {
        const string _templateFolder = @"..\..\..\..\..\Templates\RevealJs";
        const string _templateFilename = "index.html";

        public void CreatePresentation(string filepath, SlideDeck slideDeck)
        {
            var layoutStrategies = new ILayoutStrategy[Enum.GetValues(typeof(Enumerations.Layout)).Length];
            layoutStrategies[(int)Enumerations.Layout.Title] = new LayoutStrategy.Title();
            layoutStrategies[(int)Enumerations.Layout.FullPage] = new LayoutStrategy.FullPage();
            layoutStrategies[(int)Enumerations.Layout.ImageLeft] = new LayoutStrategy.ImageLeft();
            layoutStrategies[(int)Enumerations.Layout.ImageRight] = new LayoutStrategy.ImageRight();

            var slideSections = new StringBuilder();

            // Title slide
            var titleStrategy = layoutStrategies[(int)Enumerations.Layout.Title];
            slideSections.AppendLine(titleStrategy.Layout(slideDeck, null));

            // Content slides
            foreach (var slide in slideDeck.Slides)
            {
                var strategy = layoutStrategies[(int)slide.Value.Layout];
                if (strategy == null)
                    throw new NotSupportedException($"No layout strategy found for {slide.Value.Layout}");
                slideSections.AppendLine(strategy.Layout(slideDeck, slide.Value));
            }

            CopyFolder(_templateFolder, filepath);

            var templatePath = System.IO.Path.Combine(filepath, _templateFilename);
            var indexTemplate = System.IO.File.ReadAllText(templatePath);
            var content = indexTemplate.Replace("{SlideSections}", slideSections.ToString());
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