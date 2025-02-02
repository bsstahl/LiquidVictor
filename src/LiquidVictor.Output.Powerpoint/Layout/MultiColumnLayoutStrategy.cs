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
    public class MultiColumnLayoutStrategy : BaseLayoutStrategy
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

            // Get all content items ordered by key
            var contentItems = slide.ContentItems.OrderBy(ci => ci.Key).ToList();
            int columnCount = Math.Min(contentItems.Count, 3); // Maximum 3 columns
            if (columnCount == 0) return;

            // Calculate column width and spacing
            long columnWidth = 1905000L; // ~1.875 inches
            long columnSpacing = 381000L; // ~0.375 inches
            long totalWidth = (columnWidth * columnCount) + (columnSpacing * (columnCount - 1));
            long startX = (9144000L - totalWidth) / 2; // Center columns horizontally
            long startY = 2286000L; // ~2.25 inches from top

            // Create columns
            for (int i = 0; i < columnCount; i++)
            {
                var contentItem = contentItems[i].Value;
                if (contentItem == null) continue;

                if (contentItem.ContentType.StartsWith("image/"))
                {
                    // Handle image content
                    var imagePart = AddImageToSlide(slidePart, contentItem.Content, contentItem.ContentType);
                    var picture = CreatePictureShape(slidePart, contentItem.Title, contentItem.Content, imagePart);

                    // Position image in column
                    var pictureTransform = new Transform2D();
                    pictureTransform.Append(new Offset() { X = startX + (i * (columnWidth + columnSpacing)), Y = startY });
                    pictureTransform.Append(new Extents() { Cx = columnWidth, Cy = 2667000L }); // ~2.625 inches tall
                    picture.ShapeProperties.Append(pictureTransform);

                    shapeTree.AppendChild(picture);
                }
                else
                {
                    // Handle text content
                    var contentShape = CreateContentShape(contentItem.Title);

                    // Position text in column
                    var contentTransform = new Transform2D();
                    contentTransform.Append(new Offset() { X = startX + (i * (columnWidth + columnSpacing)), Y = startY });
                    contentTransform.Append(new Extents() { Cx = columnWidth, Cy = 2667000L }); // ~2.625 inches tall
                    contentShape.ShapeProperties.Append(contentTransform);

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
