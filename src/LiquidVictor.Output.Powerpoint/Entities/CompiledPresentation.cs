using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Presentation;

namespace LiquidVictor.Output.Powerpoint.Entities
{
    public class CompiledPresentation
    {
        public SlideMasterIdList SlideMasterIdList { get; set; }
        public SlideIdList SlideIdList { get; set; }
        public SlideSize SlideSize { get; set; }
        public NotesSize NotesSize { get; set; }
        public DefaultTextStyle DefaultTextStyle { get; set; }
        public List<CompiledSlide> Slides { get; set; } = new List<CompiledSlide>();
    }

    public class CompiledSlide
    {
        public uint Id { get; set; }
        public LiquidVictor.Enumerations.Layout Layout { get; set; }
        public LiquidVictor.Entities.Slide Slide { get; set; }
    }
}
