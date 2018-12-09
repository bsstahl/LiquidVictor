using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LiquidVictor.Data.Postgres
{
    [Table("slidedeckslides")]
    internal class SlideDeckSlide: SortableEntityBase
    {
        [Column("slidedeckid")]
        public Guid SlideDeckId { get; set; }
        public SlideDeck SlideDeck { get; set; }

        [Column("slideid")]
        public Guid SlideId { get; set; }
        public Slide Slide { get; set; }

    }
}
