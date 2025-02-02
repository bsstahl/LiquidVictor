using System;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using LiquidVictor.Entities;
using LiquidVictor.Output.Powerpoint.Interfaces;
using P = DocumentFormat.OpenXml.Presentation;
using D = DocumentFormat.OpenXml.Drawing;

namespace LiquidVictor.Output.Powerpoint.Layout
{
    public abstract class BaseLayoutStrategy : ILayoutStrategy
    {
        protected uint _drawingObjectId = 1;

        public abstract void Layout(SlidePart slidePart, LiquidVictor.Entities.Slide slide);

        protected P.Shape CreateTitleShape(string titleText)
        {
            var titleShape = new P.Shape();
            _drawingObjectId++;

            titleShape.NonVisualShapeProperties = new P.NonVisualShapeProperties(
                new P.NonVisualDrawingProperties() { Id = _drawingObjectId, Name = "Title" },
                new P.NonVisualShapeDrawingProperties(new ShapeLocks() { NoGrouping = true }),
                new ApplicationNonVisualDrawingProperties(new PlaceholderShape() { Type = PlaceholderValues.Title }));

            titleShape.ShapeProperties = new P.ShapeProperties();

            titleShape.TextBody = new P.TextBody(
                new BodyProperties(),
                new ListStyle(),
                new Paragraph(new Run(new D.Text() { Text = titleText })));

            return titleShape;
        }

        protected P.Shape CreateContentShape(string content, uint index = 1)
        {
            var bodyShape = new P.Shape();
            _drawingObjectId++;

            bodyShape.NonVisualShapeProperties = new P.NonVisualShapeProperties(
                new P.NonVisualDrawingProperties() { Id = _drawingObjectId, Name = "Content" },
                new P.NonVisualShapeDrawingProperties(new ShapeLocks() { NoGrouping = true }),
                new ApplicationNonVisualDrawingProperties(new PlaceholderShape() { Index = index }));

            bodyShape.ShapeProperties = new P.ShapeProperties();

            bodyShape.TextBody = new P.TextBody(
                new BodyProperties(),
                new ListStyle(),
                new Paragraph(new Run(new D.Text() { Text = content })));

            return bodyShape;
        }

        protected P.Picture CreatePictureShape(SlidePart slidePart, string name, byte[] imageBytes, ImagePart imagePart)
        {
            var picture = new P.Picture();
            _drawingObjectId++;

            var nvPicPr = new P.NonVisualPictureProperties(
                new P.NonVisualDrawingProperties() { Id = _drawingObjectId, Name = name },
                new P.NonVisualPictureDrawingProperties(new PictureLocks() { NoChangeAspect = true }),
                new ApplicationNonVisualDrawingProperties());

            var blipFill = new P.BlipFill();
            var blip = new Blip() { Embed = slidePart.GetIdOfPart(imagePart) };
            blipFill.Append(blip);
            blipFill.Append(new Stretch(new FillRectangle()));

            var spPr = new P.ShapeProperties();
            var xfrm = new Transform2D();
            xfrm.Append(new Offset() { X = 0L, Y = 0L });
            xfrm.Append(new Extents() { Cx = 5486400L, Cy = 3086400L }); // Default size, can be adjusted
            spPr.Append(xfrm);
            spPr.Append(new PresetGeometry() { Preset = ShapeTypeValues.Rectangle });

            picture.Append(nvPicPr);
            picture.Append(blipFill);
            picture.Append(spPr);

            return picture;
        }

        protected void InitializeSlide(SlidePart slidePart)
        {
            slidePart.Slide = new P.Slide(
                new CommonSlideData(
                    new ShapeTree(
                        new P.NonVisualGroupShapeProperties(
                            new P.NonVisualDrawingProperties() { Id = (UInt32Value)1U, Name = "" },
                            new P.NonVisualGroupShapeDrawingProperties(),
                            new ApplicationNonVisualDrawingProperties()),
                        new GroupShapeProperties(new TransformGroup()))),
                new ColorMapOverride(new MasterColorMapping()));
        }

        protected ImagePart AddImageToSlide(SlidePart slidePart, byte[] imageBytes, string contentType = "image/png")
        {
            var imagePart = slidePart.AddImagePart(contentType);
            using (var stream = imagePart.GetStream())
            {
                stream.Write(imageBytes, 0, imageBytes.Length);
            }
            return imagePart;
        }
    }
}
