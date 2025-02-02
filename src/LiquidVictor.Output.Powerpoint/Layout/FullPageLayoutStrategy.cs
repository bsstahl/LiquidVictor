using System;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using LiquidVictor.Entities;
using LiquidVictor.Output.Powerpoint.Entities;
using P = DocumentFormat.OpenXml.Presentation;
using D = DocumentFormat.OpenXml.Drawing;

namespace LiquidVictor.Output.Powerpoint.Layout
{
    public class FullPageLayoutStrategy : BaseLayoutStrategy
    {
        private readonly BuilderOptions _builderOptions;

        public FullPageLayoutStrategy(BuilderOptions builderOptions = null)
        {
            _builderOptions = builderOptions;
        }

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

            // Process content items
            foreach (var contentItem in slide.ContentItems.OrderBy(ci => ci.Key))
            {
                if (contentItem.Value == null) continue;

                if (contentItem.Value.ContentType.StartsWith("image/"))
                {
                    // Handle image content
                    var imagePart = AddImageToSlide(slidePart, contentItem.Value.Content, contentItem.Value.ContentType);
                    var picture = CreatePictureShape(slidePart, contentItem.Value.Title, contentItem.Value.Content, imagePart);

                    // Position image - if it's the only content and MakeSoloImagesFullScreen is true, make it full screen
                    var pictureTransform = new Transform2D();
                    if ((_builderOptions?.MakeSoloImagesFullScreen ?? false) && slide.ContentItems.Count == 1)
                    {
                        pictureTransform.Append(new Offset() { X = 0L, Y = 0L });
                        pictureTransform.Append(new Extents() { Cx = 9144000L, Cy = 6858000L }); // Full slide size
                        titleShape.Remove(); // Remove title for full screen images
                    }
                    else
                    {
                        pictureTransform.Append(new Offset() { X = 1524000L, Y = 2286000L }); // ~2.25 inches from top
                        pictureTransform.Append(new Extents() { Cx = 6096000L, Cy = 3429000L }); // ~6 inches wide, ~3.375 inches tall
                    }
                    picture.ShapeProperties.Append(pictureTransform);

                    shapeTree.AppendChild(picture);
                }
                else
                {
                    // Handle text content
                    var contentShape = CreateContentShape(contentItem.Value.Title);

                    // Position text content below title
                    var contentTransform = new Transform2D();
                    contentTransform.Append(new Offset() { X = 1524000L, Y = 2286000L }); // ~2.25 inches from top
                    contentTransform.Append(new Extents() { Cx = 6096000L, Cy = 3429000L }); // ~6 inches wide, ~3.375 inches tall
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
