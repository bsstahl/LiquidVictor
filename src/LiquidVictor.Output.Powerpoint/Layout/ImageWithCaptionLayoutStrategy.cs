using System;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using LiquidVictor.Entities;
using P = DocumentFormat.OpenXml.Presentation;
using D = DocumentFormat.OpenXml.Drawing;

namespace LiquidVictor.Output.Powerpoint.Layout
{
    public class ImageWithCaptionLayoutStrategy : BaseLayoutStrategy
    {
        public override void Layout(SlidePart slidePart, LiquidVictor.Entities.Slide slide)
        {
            InitializeSlide(slidePart);

            var titleShape = CreateTitleShape(slide.Title);

            // Position the title at the top of the slide
            var titleTransform = new Transform2D();
            titleTransform.Append(new Offset() { X = 1524000L, Y = 762000L }); // ~0.75 inches from top
            titleTransform.Append(new Extents() { Cx = 6096000L, Cy = 1143000L }); // ~6 inches wide, ~1.125 inches tall
            titleShape.ShapeProperties.Append(titleTransform);

            // Add shapes to slide
            var shapeTree = slidePart.Slide.CommonSlideData.ShapeTree;
            shapeTree.AppendChild(titleShape);

            // Get image and caption
            var imageContent = slide.ContentItems
                .OrderBy(ci => ci.Key)
                .FirstOrDefault(ci => ci.Value?.ContentType.StartsWith("image/") ?? false);

            var captionContent = slide.ContentItems
                .OrderBy(ci => ci.Key)
                .FirstOrDefault(ci => !ci.Value?.ContentType.StartsWith("image/") ?? false);

            // Add image in center if available
            if (imageContent.Value != null)
            {
                var imagePart = AddImageToSlide(slidePart, imageContent.Value.Content, imageContent.Value.ContentType);
                var picture = CreatePictureShape(slidePart, imageContent.Value.Title, imageContent.Value.Content, imagePart);

                // Position image in center
                var pictureTransform = new Transform2D();
                pictureTransform.Append(new Offset() { X = 1524000L, Y = 1905000L }); // ~1.5 inches from left, ~1.875 inches from top
                pictureTransform.Append(new Extents() { Cx = 6096000L, Cy = 2667000L }); // ~6 inches wide, ~2.625 inches tall
                picture.ShapeProperties.Append(pictureTransform);

                shapeTree.AppendChild(picture);
            }

            // Add caption below if available
            if (captionContent.Value != null)
            {
                var captionShape = CreateContentShape(captionContent.Value.Title);

                // Position caption below image
                var captionTransform = new Transform2D();
                captionTransform.Append(new Offset() { X = 1524000L, Y = 4953000L }); // ~1.5 inches from left, ~4.875 inches from top
                captionTransform.Append(new Extents() { Cx = 6096000L, Cy = 762000L }); // ~6 inches wide, ~0.75 inches tall
                captionShape.ShapeProperties.Append(captionTransform);

                // Center align the caption text
                var paragraph = captionShape.TextBody.GetFirstChild<Paragraph>();
                paragraph.ParagraphProperties = new ParagraphProperties { Alignment = TextAlignmentTypeValues.Center };

                shapeTree.AppendChild(captionShape);
            }

            // Add background if specified
            if (slide.BackgroundContent != null && slide.BackgroundContent.ContentType.StartsWith("image/"))
            {
                var backgroundPart = AddImageToSlide(slidePart, slide.BackgroundContent.Content, slide.BackgroundContent.ContentType);
                var backgroundPicture = CreatePictureShape(slidePart, "Background", slide.BackgroundContent.Content, backgroundPart);

                // Position background to fill entire slide
                var backgroundTransform = new Transform2D();
                backgroundTransform.Append(new Offset() { X = 0L, Y = 0L });
                backgroundTransform.Append(new Extents() { Cx = 9144000L, Cy = 6858000L }); // Full slide size
                backgroundPicture.ShapeProperties.Append(backgroundTransform);

                // Insert background as first child
                shapeTree.InsertAt(backgroundPicture, 0);
            }
        }
    }
}
