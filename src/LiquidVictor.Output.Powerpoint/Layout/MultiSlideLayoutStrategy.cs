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
    public class MultiSlideLayoutStrategy : BaseLayoutStrategy
    {
        public override void Layout(SlidePart slidePart, LiquidVictor.Entities.Slide slide)
        {
            // For MultiSlide, we'll create a title slide followed by content slides
            CreateTitleSlide(slidePart, slide);

            // Get all content items ordered by key
            var contentItems = slide.ContentItems.OrderBy(ci => ci.Key).ToList();
            if (!contentItems.Any()) return;

            // Create a new slide part for each content item
            foreach (var contentItem in contentItems)
            {
                if (contentItem.Value == null) continue;

                // Create a new slide part for each content item
                var presentationPart = slidePart.GetParentParts().OfType<PresentationPart>().First();
                var contentSlidePart = presentationPart.AddNewPart<SlidePart>();
                InitializeSlide(contentSlidePart);

                // Add slide ID to presentation
                var slideIdList = presentationPart.Presentation.SlideIdList;
                var slideId = new SlideId();
                slideId.RelationshipId = presentationPart.GetIdOfPart(contentSlidePart);
                slideId.Id = (uint)(slideIdList.ChildElements.Count + 256);
                slideIdList.Append(slideId);
                var shapeTree = contentSlidePart.Slide.CommonSlideData.ShapeTree;

                // Add subtitle as title for content slide
                var contentTitle = CreateTitleShape(contentItem.Value.Title);
                var titleTransform = new Transform2D();
                titleTransform.Append(new Offset() { X = 1524000L, Y = 762000L }); // ~0.75 inches from top
                titleTransform.Append(new Extents() { Cx = 6096000L, Cy = 1143000L }); // ~6 inches wide, ~1.125 inches tall
                contentTitle.ShapeProperties.Append(titleTransform);
                shapeTree.AppendChild(contentTitle);

                if (contentItem.Value.ContentType.StartsWith("image/"))
                {
                    // Handle image content
                    var imagePart = AddImageToSlide(contentSlidePart, contentItem.Value.Content, contentItem.Value.ContentType);
                    var picture = CreatePictureShape(contentSlidePart, contentItem.Value.Title, contentItem.Value.Content, imagePart);

                    // Position image in center
                    var pictureTransform = new Transform2D();
                    pictureTransform.Append(new Offset() { X = 1524000L, Y = 2286000L }); // ~1.5 inches from left, ~2.25 inches from top
                    pictureTransform.Append(new Extents() { Cx = 6096000L, Cy = 3429000L }); // ~6 inches wide, ~3.375 inches tall
                    picture.ShapeProperties.Append(pictureTransform);

                    shapeTree.AppendChild(picture);
                }
                else
                {
                    // Handle text content
                    var contentShape = CreateContentShape(contentItem.Value.Title);

                    // Position text content
                    var contentTransform = new Transform2D();
                    contentTransform.Append(new Offset() { X = 1524000L, Y = 2286000L }); // ~1.5 inches from left, ~2.25 inches from top
                    contentTransform.Append(new Extents() { Cx = 6096000L, Cy = 3429000L }); // ~6 inches wide, ~3.375 inches tall
                    contentShape.ShapeProperties.Append(contentTransform);

                    shapeTree.AppendChild(contentShape);
                }

                // Add background if specified
                if (slide.BackgroundContent != null && slide.BackgroundContent.ContentType.StartsWith("image/"))
                {
                    var backgroundPart = AddImageToSlide(contentSlidePart, slide.BackgroundContent.Content, slide.BackgroundContent.ContentType);
                    var backgroundPicture = CreatePictureShape(contentSlidePart, "Background", slide.BackgroundContent.Content, backgroundPart);

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

        private void CreateTitleSlide(SlidePart slidePart, LiquidVictor.Entities.Slide slide)
        {
            InitializeSlide(slidePart);

            var titleShape = CreateTitleShape(slide.Title);

            // Position the title in the center of the slide
            var titleTransform = new Transform2D();
            titleTransform.Append(new Offset() { X = 1524000L, Y = 2667000L }); // ~1.5 inches from left, ~2.625 inches from top
            titleTransform.Append(new Extents() { Cx = 6096000L, Cy = 1524000L }); // ~6 inches wide, ~1.5 inches tall
            titleShape.ShapeProperties.Append(titleTransform);

            // Add shapes to slide
            var shapeTree = slidePart.Slide.CommonSlideData.ShapeTree;
            shapeTree.AppendChild(titleShape);

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
