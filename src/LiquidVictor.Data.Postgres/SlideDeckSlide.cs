using System.ComponentModel.DataAnnotations.Schema;

namespace LiquidVictor.Data.Postgres;

[Table("slidedeckslides")]
sealed internal class SlideDeckSlide: SortableEntityBase
{
    [Column("slidedeckid")]
    public Guid SlideDeckId { get; set; }
    public SlideDeck SlideDeck { get; set; } = new SlideDeck();

    [Column("slideid")]
    public Guid SlideId { get; set; }
    public Slide Slide { get; set; } = new Slide();
}
