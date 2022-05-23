using System;
using System.Collections.Generic;
using System.Text;

namespace LV
{
    public class Configuration
    {
        public bool BuildTitleSlide { get; set; } = true;

        public bool MakeSoloImagesFullScreen { get; set; } = false;

        public bool SkipOutput { get; set; } = false;

        public Guid SlideDeckId { get; set; } = Guid.Empty;

        public Guid SlideId { get; set; } = Guid.Empty;

        public Guid ContentItemId { get; set; } = Guid.Empty;

        public string SourceRepoType { get; set; } = string.Empty;

        public string SourceRepoPath { get; set; } = string.Empty;

        public string OutputEngineType { get; set; } = string.Empty;

        public string TemplatePath { get; set; } = string.Empty;

        public string PresentationPath { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
    }
}
