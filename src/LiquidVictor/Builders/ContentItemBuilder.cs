using System;

namespace LiquidVictor;

public class ContentItemBuilder
{
    private readonly Entities.ContentItem _contentItem 
        = new Entities.ContentItem();

    public Entities.ContentItem Build()
    {
        return new Entities.ContentItem()
        {
            Alignment = _contentItem.Alignment,
            Content = _contentItem.Content,
            ContentType = _contentItem.ContentType,
            FileName = _contentItem.FileName,
            Id = _contentItem.Id,
            Title = _contentItem.Title
        };
    }

    public ContentItemBuilder Id(Guid value)
    {
        _contentItem.Id = value;
        return this;
    }

    public ContentItemBuilder Content(byte[] value)
    {
        _contentItem.Content = value;
        return this;
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
}