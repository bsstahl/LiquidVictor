using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LiquidVictor.Entities;

namespace LiquidVictor.Data.Postgres
{
    [Table("contentitems")]
    internal class ContentItem: EntityBase
    {
        [Column("content"), Required]
        public byte[] Content { get; set; }

        [Column("contenttype"), Required]
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
                Content = this.Content
            };
        }

        internal void FromEntity(Entities.ContentItem contentItem)
        {
            this.Id = contentItem.Id;
            this.LastModifiedDate = DateTime.UtcNow;
            this.Content = contentItem.Content;
            this.ContentType = contentItem.ContentType;
            this.Title = contentItem.Title;
            this.FileName = contentItem.FileName;
        }
    }
}
