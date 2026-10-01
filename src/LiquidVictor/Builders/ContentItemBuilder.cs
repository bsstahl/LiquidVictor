using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

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

    /// <summary>
    /// Replaces any existing tags with the supplied collection of tags
    /// </summary>
    public ContentItemBuilder Tags(IEnumerable<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var tags = values.ToList();
        tags.ForEach(ValidateTag);

        _contentItem.Tags.Clear();
        return this.AddTags(tags);
    }

    /// <summary>
    /// Adds a single tag to the item. Tags that are already present are not duplicated.
    /// </summary>
    public ContentItemBuilder AddTag(string value)
    {
        ValidateTag(value);
        if (!_contentItem.Tags.Contains(value))
            _contentItem.Tags.Add(value);

        return this;
    }

    /// <summary>
    /// Adds each of the supplied tags to the item. Tags that are already present are not duplicated.
    /// </summary>
    public ContentItemBuilder AddTags(IEnumerable<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var tags = values.ToList();
        tags.ForEach(ValidateTag);

        foreach (var tag in tags)
            this.AddTag(tag);

        return this;
    }

    /// <summary>
    /// Adds each of the supplied tags to the item. Tags that are already present are not duplicated.
    /// </summary>
    public ContentItemBuilder AddTags(params string[] values)
        => this.AddTags((IEnumerable<string>)values);

    /// <summary>
    /// Removes the specified tag from the item if it is present
    /// </summary>
    public ContentItemBuilder RemoveTag(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _contentItem.Tags.Remove(value);
        return this;
    }

    /// <summary>
    /// Removes all tags from the item
    /// </summary>
    public ContentItemBuilder ClearTags()
    {
        _contentItem.Tags.Clear();
        return this;
    }

    private static void ValidateTag(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (value.Contains(',', StringComparison.Ordinal))
            throw new ArgumentException("Tags cannot contain commas.", nameof(value));
    }
}
