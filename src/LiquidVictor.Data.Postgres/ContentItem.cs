using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LiquidVictor.Extensions;

namespace LiquidVictor.Data.Postgres;

[Table("contentitems")]
sealed internal class ContentItem: EntityBase
{
    [Column("encodedcontent"), Required]
    public string EncodedContent { get; set; } = string.Empty;

    [Column("contenttype"), MaxLength(100), Required]
    public string ContentType { get; set; } = "text/plain";

    [Column("title"), MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Column("filename"), MaxLength(260)]
    public string FileName { get; set; } = string.Empty;

    [Column("alignment"), MaxLength(50)]
    public string Alignment { get; set; } = string.Empty;

    [Column("tags")]
    public string Tags { get; set; } = string.Empty;

    internal Entities.ContentItem AsEntity()
    {
        var entity = new Entities.ContentItem()
        {
            Id = this.Id,
            ContentType = this.ContentType,
            FileName = this.FileName,
            Title = this.Title,
            Alignment = this.Alignment,
            Content = this.EncodedContent.FromBase64String()
        };

        if (!string.IsNullOrEmpty(this.Tags))
        {
            var tags = this.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var tag in tags)
                entity.Tags.Add(tag);
        }

        return entity;
    }

    internal void FromEntity(Entities.ContentItem contentItem)
    {
        this.Id = contentItem.Id;
        this.LastModifiedDate = DateTime.UtcNow;
        this.ContentType = contentItem.ContentType;
        this.Title = contentItem.Title;
        this.FileName = contentItem.FileName;
        this.Alignment = contentItem.Alignment;
        this.EncodedContent = contentItem.Content.AsBase64String();
        this.Tags = contentItem.Tags.Count > 0 ? string.Join(",", contentItem.Tags) : string.Empty;
    }
}
