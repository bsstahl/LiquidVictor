﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using LiquidVictor.Entities;
using LiquidVictor.Interfaces;
using LiquidVictor.Output.Powerpoint.Entities;
using LiquidVictor.Output.Powerpoint.Interfaces;
using LiquidVictor.Output.Powerpoint.Layout;

namespace LiquidVictor.Output.Powerpoint.Generator
{
    public class Engine : IPresentationBuilder
    {
        private readonly ILayoutStrategy[] _layoutStrategies;
        private readonly BuilderOptions _builderOptions;

        public Engine(BuilderOptions builderOptions)
        {
            _builderOptions = builderOptions;
            _layoutStrategies = CreateLayoutStrategies();
        }

        public Engine() : this(new BuilderOptions())
        {
        }

        private ILayoutStrategy[] CreateLayoutStrategies()
        {
            var strategies = new ILayoutStrategy[Enum.GetValues(typeof(LiquidVictor.Enumerations.Layout)).Length];
            strategies[(int)LiquidVictor.Enumerations.Layout.Title] = new TitleLayoutStrategy();
            strategies[(int)LiquidVictor.Enumerations.Layout.FullPage] = new FullPageLayoutStrategy(_builderOptions);
            strategies[(int)LiquidVictor.Enumerations.Layout.FullPageFragments] = new FullPageLayoutStrategy(_builderOptions); // Use FullPage for fragments since animations aren't fully supported
            strategies[(int)LiquidVictor.Enumerations.Layout.ImageLeft] = new ImageLeftLayoutStrategy();
            strategies[(int)LiquidVictor.Enumerations.Layout.ImageRight] = new ImageRightLayoutStrategy();
            strategies[(int)LiquidVictor.Enumerations.Layout.ImageWithCaption] = new ImageWithCaptionLayoutStrategy();
            strategies[(int)LiquidVictor.Enumerations.Layout.MultiColumn] = new MultiColumnLayoutStrategy();
            strategies[(int)LiquidVictor.Enumerations.Layout.MultiSlide] = new MultiSlideLayoutStrategy();
            return strategies;
        }

        private CompiledPresentation CompilePresentationInternal(SlideDeck slideDeck)
        {
            var compiled = new CompiledPresentation
            {
                SlideMasterIdList = new SlideMasterIdList(new SlideMasterId() { Id = (UInt32Value)2147483648U, RelationshipId = "rId1" }),
                SlideIdList = new SlideIdList(),
                SlideSize = new SlideSize() { Cx = 9144000, Cy = 6858000, Type = SlideSizeValues.Screen4x3 },
                NotesSize = new NotesSize() { Cx = 6858000, Cy = 9144000 },
                DefaultTextStyle = new DefaultTextStyle()
            };

            uint slideId = 256;

            // Add title slide if enabled and title exists
            if ((_builderOptions?.BuildTitleSlide ?? true) && slideDeck.Title != null)
            {
                var titleSlide = new LiquidVictor.Entities.Slide
                {
                    Title = slideDeck.Title,
                    Layout = LiquidVictor.Enumerations.Layout.Title
                };

                if (!string.IsNullOrEmpty(slideDeck.Presenter))
                {
                    titleSlide.ContentItems.Add(new KeyValuePair<int, ContentItem>(1, new ContentItem 
                    { 
                        Title = slideDeck.Presenter,
                        ContentType = "text/plain"
                    }));
                }

                compiled.Slides.Add(new CompiledSlide
                {
                    Id = slideId++,
                    Layout = LiquidVictor.Enumerations.Layout.Title,
                    Slide = titleSlide
                });
            }

            // Add content slides
            foreach (var slide in slideDeck.Slides.OrderBy(s => s.Key))
            {
                var layout = slide.Value.Layout;

                // Check if we should make solo images full screen
                if (_builderOptions?.MakeSoloImagesFullScreen == true && 
                    slide.Value.ContentItems.Count == 1 && 
                    slide.Value.ContentItems.First().Value?.ContentType.StartsWith("image/") == true)
                {
                    layout = LiquidVictor.Enumerations.Layout.FullPage;
                }

                compiled.Slides.Add(new CompiledSlide
                {
                    Id = slideId++,
                    Layout = layout,
                    Slide = slide.Value
                });
            }

            return compiled;
        }

        public void CompilePresentation(SlideDeck slideDeck)
        {
            // This method exists to satisfy the interface
            // The actual compilation work is done in CompilePresentationInternal
            CompilePresentationInternal(slideDeck);
        }

        public void CreatePresentation(string filepath, SlideDeck slideDeck)
        {
            // Ensure the filepath has .pptx extension
            if (!filepath.EndsWith(".pptx", StringComparison.OrdinalIgnoreCase))
            {
                filepath = Path.ChangeExtension(filepath, ".pptx");
            }

            var compiled = CompilePresentationInternal(slideDeck);

            using (PresentationDocument presentationDoc = PresentationDocument.Create(filepath, PresentationDocumentType.Presentation))
            {
                PresentationPart presentationPart = presentationDoc.AddPresentationPart();
                presentationPart.Presentation = new Presentation();

                // Add presentation structure
                presentationPart.Presentation.Append(
                    compiled.SlideMasterIdList,
                    compiled.SlideIdList,
                    compiled.SlideSize,
                    compiled.NotesSize,
                    compiled.DefaultTextStyle
                );

                // Create slides
                foreach (var compiledSlide in compiled.Slides)
                {
                    SlidePart slidePart = presentationPart.AddNewPart<SlidePart>();
                    var layoutStrategy = _layoutStrategies[(int)compiledSlide.Layout] ?? _layoutStrategies[(int)LiquidVictor.Enumerations.Layout.FullPage];
                    layoutStrategy.Layout(slidePart, compiledSlide.Slide);

                    compiled.SlideIdList.Append(new SlideId() 
                    { 
                        Id = (UInt32Value)compiledSlide.Id, 
                        RelationshipId = presentationPart.GetIdOfPart(slidePart) 
                    });
                }
            }
        }
    }
}
