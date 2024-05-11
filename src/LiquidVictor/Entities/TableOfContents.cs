using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Entities;

public class TableOfContents(Guid id, string title, string subTitle, string presenter, 
    Uri? url, IEnumerable<TableOfContentsEntry> entries)
{
    public Guid SlideDeckId { get; set; } = id;
    public string SlideDeckTitle { get; set; } = title;
    public string SlideDeckSubTitle { get; set; } = subTitle;
    public string SlideDeckPresenter { get; set; } = presenter;

    public Uri? SlideDeckUrl { get; set; } = url;
    
    public IEnumerable<TableOfContentsEntry> Entries { get; set; } = entries;
}
