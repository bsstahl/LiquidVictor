using System;

namespace LiquidVictor.Data.YamlFile;

public class ChildId
{
    public Guid Id { get; set; }
    public string Title { get; set; }

    public ChildId(Guid id, string title)
    {
        this.Id = id;
        this.Title = title;
    }
}
