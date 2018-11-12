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
            var slideSections = new StringBuilder();

            // Title slide
            slideSections.AppendLine($"<section>{Markdig.Markdown.ToHtml($"# {slideDeck.Title}\r\n## {slideDeck.SubTitle}")}</section>\r\n");

            // Content slides
            foreach (var slide in slideDeck.Slides)
            {
                // TODO: Pull out into a strategy
                if (slide.Value.Layout == Enumerations.Layout.FullPage)
                    slideSections.AppendLine($"<section><h1>{slide.Value.Title}</h1>{Markdig.Markdown.ToHtml(slide.Value.ContentText)}</section>\r\n");
                else if (slide.Value.Layout == Enumerations.Layout.ImageRight)
                {
                    slideSections.AppendLine("<section>");
                    slideSections.AppendLine($"<h1>{slide.Value.Title}</h1>");
                    slideSections.Append("<table><tr>");
                    slideSections.AppendLine($"<td style=\"vertical-align:top;\">{Markdig.Markdown.ToHtml(slide.Value.ContentText)}</td>");

                    var image = slide.Value.PrimaryImage;
                    if (image != null)
                        slideSections.AppendLine($"<td width=\"60%\"><img alt=\"{image.Name}\" src=\"data:{image.ImageFormat};base64,{image.Content.ToBase64()}\" /></td>");

                    slideSections.Append("</tr></table>");
                    slideSections.AppendLine("</section>");
                }
                else if (slide.Value.Layout == Enumerations.Layout.ImageLeft)
                {
                    slideSections.AppendLine("<section>");
                    slideSections.AppendLine($"<h1>{slide.Value.Title}</h1>");
                    slideSections.Append("<table><tr>");

                    var image = slide.Value.PrimaryImage;
                    if (image != null)
                        slideSections.AppendLine($"<td width=\"60%\"><img alt=\"{image.Name}\" src=\"data:{image.ImageFormat};base64,{image.Content.ToBase64()}\" /></td>");

                    slideSections.AppendLine($"<td style=\"vertical-align:top;\">{Markdig.Markdown.ToHtml(slide.Value.ContentText)}</td>");

                    slideSections.Append("</tr></table>");
                    slideSections.AppendLine("</section>");
                }
                else
                    throw new NotImplementedException();
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