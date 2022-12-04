using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Entities
{
    public class TableOfContents
    {
        public Guid SlideDeckId { get; set; }
        public string SlideDeckTitle { get; set; } = string.Empty;
        public string SlideDeckSubTitle { get; set; } = string.Empty;
        public string SlideDeckPresenter { get; set; } = string.Empty;
        public string SlideDeckUrl { get; set; } = string.Empty;
        public IEnumerable<TableOfContentsEntry> Entries { get; set; } = Array.Empty<TableOfContentsEntry>();
    }
}
