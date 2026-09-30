namespace LVImport.Test;

public class WriteRepositoryFactory_Create_Should
{
    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("YamlFile", true)]
    [InlineData("yamlfile", true)]
    [InlineData("Postgres", true)]
    [InlineData("PostgreSQL", true)]
    [InlineData("JsonFileSystem", false)]
    [InlineData("", false)]
    public void IdentifySupportedRepositoryTypes(string repositoryType, bool expected)
    {
        Assert.Equal(expected, WriteRepositoryFactory.IsSupported(repositoryType));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void CreateAYamlFileRepository()
    {
        var repository = WriteRepositoryFactory.Create("YamlFile", Path.GetTempPath());
        Assert.IsType<LiquidVictor.Data.YamlFile.SlideDeckWriteRepository>(repository);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowForAnUnsupportedRepositoryType()
    {
        Assert.Throws<NotSupportedException>(() => WriteRepositoryFactory.Create("JsonFileSystem", Path.GetTempPath()));
    }
}
