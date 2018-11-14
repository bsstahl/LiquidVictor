using LiquidVictor.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Entities
{
    public class Slide
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string[] ContentText { get; set; }
        public Layout Layout { get; set; }

        // public Interfaces.IPresentationImage BackgroundImage { get; set; }

        // public string Notes { get; set; }

        public PresentationImage PrimaryImage { get; set; }
    }
}
