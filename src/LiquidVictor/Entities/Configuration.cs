using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Entities
{
    public class Configuration
    {
        public bool BuildTitleSlide { get; set; } = true;

        public bool MakeSoloImagesFullScreen { get; set; } = false;

        public bool SkipOutput { get; set; } = false;
    }
}
