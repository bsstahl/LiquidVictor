using LiquidVictor.Entities;
using LiquidVictor.Interfaces;
using System;
using System.Text;
using LiquidVictor.Extensions;

namespace LiquidVictor.Output.RevealJs
{
    public class Engine : IPresentationBuilder
    {
        const string _templatePath = @"..\..\..\..\..\Templates\RevealJs\index.html";

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
                        slideSections.AppendLine($"<td width=\"60%\"><img alt=\"{image.Name}\" src=\"data:{image.Content};base64,{image.Content.ToBase64()}\" /></td>");

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
                        slideSections.AppendLine($"<td width=\"60%\"><img alt=\"{image.Name}\" src=\"data:{image.Content};base64,{image.Content.ToBase64()}\" /></td>");

                    slideSections.AppendLine($"<td style=\"vertical-align:top;\">{Markdig.Markdown.ToHtml(slide.Value.ContentText)}</td>");

                    slideSections.Append("</tr></table>");
                    slideSections.AppendLine("</section>");
                }
                else
                    throw new NotImplementedException();
            }

            var indexTemplate = System.IO.File.ReadAllText(_templatePath);
            var content = indexTemplate.Replace("{SlideSections}", slideSections.ToString());


            System.IO.Directory.CreateDirectory(filepath);
            string indexPath = System.IO.Path.Combine(filepath, "index.html");
            System.IO.File.WriteAllText(indexPath, content);
        }
    }
}
