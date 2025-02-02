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
    public class ImageLeftLayoutStrategy : BaseLayoutStrategy
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

            // Get image and content
            var imageContent = slide.ContentItems
                .OrderBy(ci => ci.Key)
                .FirstOrDefault(ci => ci.Value?.ContentType.StartsWith("image/") ?? false);

            var textContent = slide.ContentItems
                .OrderBy(ci => ci.Key)
                .FirstOrDefault(ci => !ci.Value?.ContentType.StartsWith("image/") ?? false);

            // Add image on the left if available
            if (imageContent.Value != null)
            {
                var imagePart = AddImageToSlide(slidePart, imageContent.Value.Content, imageContent.Value.ContentType);
                var picture = CreatePictureShape(slidePart, imageContent.Value.Title, imageContent.Value.Content, imagePart);

                // Position image on the left side
                var pictureTransform = new Transform2D();
                pictureTransform.Append(new Offset() { X = 762000L, Y = 2286000L }); // ~0.75 inches from left, ~2.25 inches from top
                pictureTransform.Append(new Extents() { Cx = 3048000L, Cy = 3429000L }); // ~3 inches wide, ~3.375 inches tall
                picture.ShapeProperties.Append(pictureTransform);

                shapeTree.AppendChild(picture);
            }

            // Add text content on the right if available
            if (textContent.Value != null)
            {
                var contentShape = CreateContentShape(textContent.Value.Title);

                // Position text content on the right side
                var contentTransform = new Transform2D();
                contentTransform.Append(new Offset() { X = 4572000L, Y = 2286000L }); // ~4.5 inches from left, ~2.25 inches from top
                contentTransform.Append(new Extents() { Cx = 3048000L, Cy = 3429000L }); // ~3 inches wide, ~3.375 inches tall
                contentShape.ShapeProperties.Append(contentTransform);

                shapeTree.AppendChild(contentShape);
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
