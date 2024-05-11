using System;
using System.Collections.Generic;

namespace LiquidVictor.Entities
{
    public class TableOfContentsEntry
    {
        public Guid SlideId { get; set; } = Guid.Empty;
        public int SlideIndexInDeck { get; set; }
        public string Title { get; set; } = string.Empty;
        public IEnumerable<string> ContentItemTitles { get; set; } = [];
    }
}
