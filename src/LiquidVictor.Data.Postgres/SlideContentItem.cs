using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LiquidVictor.Data.Postgres
{
    [Table("slidecontentitems")]
    internal class SlideContentItem: SortableEntityBase
    {
        [Column("slideid")]
        public Guid SlideId { get; set; }
        public Slide Slide { get; set; }

        [Column("contentitemid")]
        public Guid ContentItemId { get; set; }
        public ContentItem ContentItem { get; set; }
    }
}
