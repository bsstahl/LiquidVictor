using LiquidVictor.Entities;
using System;
using System.Collections.Generic;

namespace LiquidVictor.Builders;

public class ContentItemBuilder(ContentItem? value)
{
    private readonly Entities.ContentItem _contentItem 
        = value ?? new ContentItem();

    public ContentItemBuilder()
        : this(new Entities.ContentItem())
    { }

    public Entities.ContentItem Build()
    {
        var result = new Entities.ContentItem()
        {
            Alignment = _contentItem.Alignment,
            Content = _contentItem.Content,
            ContentType = _contentItem.ContentType,
            FileName = _contentItem.FileName,
            Id = _contentItem.Id.Equals(Guid.Empty) 
                ? Guid.NewGuid() 
                : _contentItem.Id,
            Title = _contentItem.Title
        };

        foreach (var tag in _contentItem.Tags)
            result.Tags.Add(tag);

        return result;
    }

    public ContentItemBuilder Id(Guid value)
    {
        _contentItem.Id = value;
        return this;
    }

    /// <summary>
    /// Defines the content of the item. Use this overload for image and other binary types
    /// </summary>
    public ContentItemBuilder Content(byte[] value)
    {
        _contentItem.Content = value;
        return this;
    }

    /// <summary>
    /// Defines the content of the item. Use this overload for text data
    /// </summary>
    public ContentItemBuilder Content(string value)
    {
        return this.Content(System.Text.Encoding.UTF8.GetBytes(value));
    }

    public ContentItemBuilder ContentType(string value)
    {
        _contentItem.ContentType = value;
        return this;
    }

    public ContentItemBuilder FileName(string value)
    {
        _contentItem.FileName = value;
        return this;
    }

    public ContentItemBuilder Title(string value)
    {
        _contentItem.Title = value;
        return this;
    }

    public ContentItemBuilder Alignment(string value) 
    {
        _contentItem.Alignment = value;
        return this;
    }

    public ContentItemBuilder Tags(IEnumerable<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        _contentItem.Tags.Clear();
        foreach (var value in values)
            _contentItem.Tags.Add(value);

        return this;
    }
}
