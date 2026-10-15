using Microsoft.Extensions.Configuration;
using LV;

namespace LiquidVictor.Data.YamlFile.Test;

public class ArgumentExtensions_Parse_Should
{
    [Fact]
    [Trait("Category", "Unit")]
    public void DisableResourcesSlideWithNoResourcesFlag()
    {
        var (_, config) = new[] { "--NORESOURCES" }.Parse();

        Assert.False(config.BuildResourcesSlide);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ReadResourcesSlideDefaultFromConfiguration()
    {
        var defaults = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["BuildResourcesSlide"] = "false" })
            .Build();

        var (_, config) = Array.Empty<string>().Parse(defaults);

        Assert.False(config.BuildResourcesSlide);
    }
}
