using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LiquidVictor.Entities;

public class ContentItem
{
    public Guid Id { get; set; }

#pragma warning disable CA1819 // Properties should not return arrays
    public byte[] Content { get; set; } = [];
#pragma warning restore CA1819 // Properties should not return arrays
    
    public string ContentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Alignment { get; set; } = string.Empty;
    public Collection<string> Tags { get; } = [];


    public ContentItem()
    { }

    public ContentItem(Guid id, byte[] content, string contentType, string fileName, 
        string title, string alignment, IEnumerable<string>? tags = null)
    {
        this.Id = id;
        this.Content = content;
        this.ContentType = contentType;
        this.FileName = fileName;
        this.Title = title;
        this.Alignment = alignment;
        if (tags is not null)
        {
            foreach (var tag in tags)
                this.Tags.Add(tag);
        }
    }

    public ContentItem Clone(bool createNewId = false)
    {
        return new ContentItem(createNewId ? Guid.NewGuid() : this.Id, this.Content, 
            this.ContentType, this.FileName, this.Title, this.Alignment, this.Tags)
        { };
    }
}
