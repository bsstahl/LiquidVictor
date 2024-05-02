using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Entities
{
    public class ContentItem
    {
        public Guid Id { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Alignment { get; set; }


        public ContentItem Clone(bool createNewId = false)
        {
            return new ContentItem()
            {
                Id = createNewId ? Guid.NewGuid() : this.Id,
                Title = this.Title,
                Content = this.Content,
                ContentType = this.ContentType,
                FileName = this.FileName,
                Alignment = this.Alignment
            };
        }
    }
}
