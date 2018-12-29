using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LiquidVictor.Extensions;

namespace LiquidVictor.Data.Postgres
{
    [Table("contentitems")]
    internal class ContentItem: EntityBase
    {
        [Column("encodedcontent"), Required]
        public string EncodedContent { get; set; }

        [Column("contenttype"), MaxLength(100), Required]
        public string ContentType { get; set; }

        [Column("title"), MaxLength(200)]
        public string Title { get; set; }

        [Column("filename"), MaxLength(260)]
        public string FileName { get; set; }

        internal Entities.ContentItem AsEntity()
        {
            return new Entities.ContentItem()
            {
                Id = this.Id,
                ContentType = this.ContentType,
                FileName = this.FileName,
                Title = this.Title,
                Content = this.EncodedContent.FromBase64String()
            };
        }

        internal void FromEntity(Entities.ContentItem contentItem)
        {
            this.Id = contentItem.Id;
            this.LastModifiedDate = DateTime.UtcNow;
            this.ContentType = contentItem.ContentType;
            this.Title = contentItem.Title;
            this.FileName = contentItem.FileName;
            this.EncodedContent = contentItem.Content.AsBase64String();
        }
    }
}
