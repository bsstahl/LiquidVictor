using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Data.JsonFileSystem
{
    internal class ContentItem
    {
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string EncodedContent { get; set; }

    }
}
