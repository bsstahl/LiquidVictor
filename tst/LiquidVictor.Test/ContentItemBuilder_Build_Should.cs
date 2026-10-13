using System.Diagnostics.CodeAnalysis;
using LiquidVictor.Builders;

namespace LiquidVictor.Test;

[ExcludeFromCodeCoverage]
public class ContentItemBuilder_Build_Should
{
    [Fact]
    [Trait("Category", "Unit")]
    public void ReturnAContentItemWithAllPersistedValuesSet()
    {
        var id = Guid.NewGuid();
        var content = new byte[] { 1, 2, 3 };
        var contentType = "image/png";
        var fileName = $"{string.Empty.GetRandom()}.png";
        var title = string.Empty.GetRandom();
        var alignment = "center";

        var target = new ContentItemBuilder()
            .Id(id)
            .Content(content)
            .ContentType(contentType)
            .FileName(fileName)
            .Title(title)
            .Alignment(alignment)
            .AddTag("character")
            .AddTag("emotion")
            .Build();

        Assert.Equal(id, target.Id);
        Assert.Equal(content, target.Content);
        Assert.Equal(contentType, target.ContentType);
        Assert.Equal(fileName, target.FileName);
        Assert.Equal(title, target.Title);
        Assert.Equal(alignment, target.Alignment);
        Assert.Equal(["character", "emotion"], target.Tags);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ReturnAContentItemWithNoTagsIfNoneAreAdded()
    {
        var target = new ContentItemBuilder().Build();
        Assert.Empty(target.Tags);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AssignANewIdIfNoneIsSpecified()
    {
        var target = new ContentItemBuilder().Build();
        Assert.NotEqual(Guid.Empty, target.Id);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void NotDuplicateATagThatIsAddedMoreThanOnce()
    {
        var target = new ContentItemBuilder()
            .AddTag("diagram")
            .AddTag("diagram")
            .AddTags("diagram", "architecture")
            .Build();

        Assert.Equal(["diagram", "architecture"], target.Tags);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void IncludeAllTagsAddedAsACollection()
    {
        IEnumerable<string> tags = ["deck", "background"];

        var target = new ContentItemBuilder()
            .AddTag("image")
            .AddTags(tags)
            .Build();

        Assert.Equal(["image", "deck", "background"], target.Tags);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ReplaceExistingTagsWhenTagsIsCalled()
    {
        var target = new ContentItemBuilder()
            .AddTag("old")
            .Tags(["new1", "new2"])
            .Build();

        Assert.Equal(["new1", "new2"], target.Tags);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ReturnNoTagsIfTagsAreCleared()
    {
        var target = new ContentItemBuilder()
            .AddTags("character", "emotion")
            .ClearTags()
            .Build();

        Assert.Empty(target.Tags);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void IncludeTagsAddedAfterTagsAreCleared()
    {
        var target = new ContentItemBuilder()
            .AddTag("character")
            .ClearTags()
            .AddTag("diagram")
            .Build();

        Assert.Equal(["diagram"], target.Tags);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void NotIncludeATagThatHasBeenRemoved()
    {
        var target = new ContentItemBuilder()
            .AddTags("character", "emotion", "overworked")
            .RemoveTag("emotion")
            .RemoveTag("not-present")
            .Build();

        Assert.Equal(["character", "overworked"], target.Tags);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void PreserveTagsFromTheSourceContentItem()
    {
        var source = new Entities.ContentItem();
        source.Tags.Add("character");

        var target = new ContentItemBuilder(source)
            .AddTag("emotion")
            .Build();

        Assert.Equal(["character", "emotion"], target.Tags);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ReturnIndependentTagCollectionsFromEachBuild()
    {
        var builder = new ContentItemBuilder().AddTag("character");

        var first = builder.Build();
        builder.AddTag("emotion");
        var second = builder.Build();

        Assert.Equal(["character"], first.Tags);
        Assert.Equal(["character", "emotion"], second.Tags);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("a,b")]
    public void ThrowIfAnInvalidTagIsAdded(string value)
    {
        var builder = new ContentItemBuilder();
        Assert.ThrowsAny<ArgumentException>(() => builder.AddTag(value));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowIfANullTagIsAdded()
    {
        var builder = new ContentItemBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.AddTag(null!));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void NotAddAnyTagsIfAnyTagInTheCollectionIsInvalid()
    {
        var builder = new ContentItemBuilder().AddTag("existing");

        Assert.ThrowsAny<ArgumentException>(() => builder.AddTags("valid", " "));
        Assert.ThrowsAny<ArgumentException>(() => builder.Tags(["valid", "a,b"]));

        Assert.Equal(["existing"], builder.Build().Tags);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowIfANullTagCollectionIsSupplied()
    {
        var builder = new ContentItemBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.AddTags((IEnumerable<string>)null!));
        Assert.Throws<ArgumentNullException>(() => builder.Tags(null!));
    }
}
