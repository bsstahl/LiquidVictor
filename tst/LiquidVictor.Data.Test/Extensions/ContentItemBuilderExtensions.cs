using System.Text;
using TestHelperExtensions;
using LiquidVictor.Builders;

namespace LiquidVictor.Data.Test.Extensions;

public static class ContentItemBuilderExtensions
{
    public static ContentItemBuilder Content(this ContentItemBuilder builder, string value)
    {
        return builder.Content(Encoding.UTF8.GetBytes(value));
    }

    public static ContentItemBuilder UseRandomValues(this ContentItemBuilder builder)
    {
        var alignments = new string[] { "left", "center", "right" };

        return builder
            .Id(Guid.NewGuid())
            .Alignment(alignments.GetRandom())
            .Content(string.Empty.GetRandom())
            .ContentType("text/plain")
            .FileName($"{string.Empty.GetRandom()}.txt")
            .Title(string.Empty.GetRandom());
    }
}
