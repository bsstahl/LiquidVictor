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
    public class FullPageFragmentsLayoutStrategy : BaseLayoutStrategy
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

            // Get all content items ordered by key (order determines animation sequence)
            var contentItems = slide.ContentItems.OrderBy(ci => ci.Key).ToList();
            if (!contentItems.Any()) return;

            // Calculate vertical spacing for fragments
            long startY = 2286000L; // ~2.25 inches from top
            long itemHeight = 762000L; // ~0.75 inches per item
            long itemSpacing = 381000L; // ~0.375 inches between items

            // Create fragments
            for (int i = 0; i < contentItems.Count; i++)
            {
                var contentItem = contentItems[i].Value;
                if (contentItem == null) continue;

                if (contentItem.ContentType.StartsWith("image/"))
                {
                    // Handle image content
                    var imagePart = AddImageToSlide(slidePart, contentItem.Content, contentItem.ContentType);
                    var picture = CreatePictureShape(slidePart, contentItem.Title, contentItem.Content, imagePart);

                    // Position image
                    var pictureTransform = new Transform2D();
                    pictureTransform.Append(new Offset() { X = 1524000L, Y = startY + (i * (itemHeight + itemSpacing)) });
                    pictureTransform.Append(new Extents() { Cx = 6096000L, Cy = itemHeight });
                    picture.ShapeProperties.Append(pictureTransform);

                    // No animations - just stack content vertically
                    shapeTree.AppendChild(picture);
                }
                else
                {
                    // Handle text content
                    var contentShape = CreateContentShape(contentItem.Title);

                    // Position text content
                    var contentTransform = new Transform2D();
                    contentTransform.Append(new Offset() { X = 1524000L, Y = startY + (i * (itemHeight + itemSpacing)) });
                    contentTransform.Append(new Extents() { Cx = 6096000L, Cy = itemHeight });
                    contentShape.ShapeProperties.Append(contentTransform);

                    // No animations - just stack content vertically
                    shapeTree.AppendChild(contentShape);
                }
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
