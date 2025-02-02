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
    public class TitleLayoutStrategy : BaseLayoutStrategy
    {
        public override void Layout(SlidePart slidePart, LiquidVictor.Entities.Slide slide)
        {
            InitializeSlide(slidePart);

            var titleShape = CreateTitleShape(slide.Title);

            // Add content as subtitle if available
            P.Shape subtitleShape = null;
            var firstContent = slide.ContentItems.OrderBy(ci => ci.Key).FirstOrDefault();
            if (firstContent.Value != null && !string.IsNullOrEmpty(firstContent.Value.Title))
            {
                subtitleShape = new P.Shape();
                _drawingObjectId++;

                subtitleShape.NonVisualShapeProperties = new P.NonVisualShapeProperties(
                    new P.NonVisualDrawingProperties() { Id = _drawingObjectId, Name = "Subtitle" },
                    new P.NonVisualShapeDrawingProperties(new ShapeLocks() { NoGrouping = true }),
                    new ApplicationNonVisualDrawingProperties(new PlaceholderShape() { Type = PlaceholderValues.Body }));

                subtitleShape.ShapeProperties = new P.ShapeProperties();

                subtitleShape.TextBody = new P.TextBody(
                    new BodyProperties(),
                    new ListStyle(),
                    new Paragraph(new Run(new D.Text() { Text = firstContent.Value.Title })));
            }

            // Position the title in the upper portion of the slide
            var titleTransform = new Transform2D();
            titleTransform.Append(new Offset() { X = 1524000L, Y = 1524000L }); // ~1.5 inches from top
            titleTransform.Append(new Extents() { Cx = 6096000L, Cy = 1524000L }); // ~6 inches wide, ~1.5 inches tall
            titleShape.ShapeProperties.Append(titleTransform);

            // Position the subtitle below the title if it exists
            if (subtitleShape != null)
            {
                var subtitleTransform = new Transform2D();
                subtitleTransform.Append(new Offset() { X = 1524000L, Y = 3048000L }); // ~3 inches from top
                subtitleTransform.Append(new Extents() { Cx = 6096000L, Cy = 1524000L }); // ~6 inches wide, ~1.5 inches tall
                subtitleShape.ShapeProperties.Append(subtitleTransform);
            }

            // Add shapes to slide
            var shapeTree = slidePart.Slide.CommonSlideData.ShapeTree;
            shapeTree.AppendChild(titleShape);
            if (subtitleShape != null)
            {
                shapeTree.AppendChild(subtitleShape);
            }
        }
    }
}
