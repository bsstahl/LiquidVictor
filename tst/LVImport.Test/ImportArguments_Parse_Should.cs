namespace LVImport.Test;

public class ImportArguments_Parse_Should
{
    [Fact]
    [Trait("Category", "Unit")]
    public void ParseAllSupportedArguments()
    {
        var options = ImportArguments.Parse([
            "deck.pptx",
            "-SourceRepoType:Postgres",
            "-SourceRepoPath:Host=localhost",
            "-Slides:1,3-4",
            "-Title:My Deck",
            "--SkipOutput",
            "--Verbose"]);

        Assert.Equal("deck.pptx", options.PowerpointPath);
        Assert.Equal("Postgres", options.SourceRepoType);
        Assert.Equal("Host=localhost", options.SourceRepoPath);
        Assert.Equal([1, 3, 4], options.SlideNumbers);
        Assert.Equal("My Deck", options.Title);
        Assert.True(options.SkipOutput);
        Assert.True(options.Verbose);
        Assert.False(options.ShowHelp);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ApplyDefaultsForOptionalArguments()
    {
        var options = ImportArguments.Parse(["deck.pptx", "-SourceRepoPath:/repo"]);

        Assert.Equal(ImportOptions.DefaultRepositoryType, options.SourceRepoType);
        Assert.Empty(options.SlideNumbers);
        Assert.Equal(string.Empty, options.Title);
        Assert.False(options.SkipOutput);
        Assert.False(options.Verbose);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void TreatSwitchNamesCaseInsensitivelyButPreserveValueCase()
    {
        var options = ImportArguments.Parse(["Deck.PPTX", "-sourcerepotype:yamlfile", "-sourcerepopath:/Some/Repo", "-title:Mixed Case"]);

        Assert.Equal("Deck.PPTX", options.PowerpointPath);
        Assert.Equal("yamlfile", options.SourceRepoType);
        Assert.Equal("/Some/Repo", options.SourceRepoPath);
        Assert.Equal("Mixed Case", options.Title);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("--help")]
    [InlineData("-h")]
    [InlineData("-?")]
    [InlineData("/?")]
    public void ShowHelpWithoutRequiringOtherArguments(string arg)
    {
        var options = ImportArguments.Parse([arg]);
        Assert.True(options.ShowHelp);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AllowSkipOutputWithoutARepositoryPath()
    {
        var options = ImportArguments.Parse(["deck.pptx", "--SkipOutput"]);
        Assert.True(options.SkipOutput);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowWhenNoPowerpointPathIsProvided()
    {
        Assert.Throws<ArgumentException>(() => ImportArguments.Parse(["-SourceRepoPath:/repo"]));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowWhenNoRepositoryPathIsProvided()
    {
        Assert.Throws<ArgumentException>(() => ImportArguments.Parse(["deck.pptx"]));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowWhenMultiplePowerpointPathsAreProvided()
    {
        Assert.Throws<ArgumentException>(() => ImportArguments.Parse(["one.pptx", "two.pptx", "-SourceRepoPath:/repo"]));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowWhenAnUnknownSwitchIsProvided()
    {
        Assert.Throws<ArgumentException>(() => ImportArguments.Parse(["deck.pptx", "-SourceRepoPath:/repo", "-Bogus:1"]));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ThrowWhenTheRepositoryTypeIsNotSupported()
    {
        Assert.Throws<ArgumentException>(() => ImportArguments.Parse(["deck.pptx", "-SourceRepoPath:/repo", "-SourceRepoType:JsonFileSystem"]));
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("1", new[] { 1 })]
    [InlineData("3,1,2", new[] { 1, 2, 3 })]
    [InlineData("2-4", new[] { 2, 3, 4 })]
    [InlineData(" 1 , 5 - 6 ,1", new[] { 1, 5, 6 })]
    [InlineData("7-7", new[] { 7 })]
    public void ParseSlideNumbersAndRanges(string value, int[] expected)
    {
        Assert.Equal(expected, ImportArguments.ParseSlideNumbers(value));
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("")]
    [InlineData(",")]
    [InlineData("0")]
    [InlineData("a")]
    [InlineData("-1")]
    [InlineData("4-2")]
    [InlineData("1-2-3")]
    [InlineData("1.5")]
    public void ThrowForInvalidSlideNumbers(string value)
    {
        Assert.Throws<ArgumentException>(() => ImportArguments.ParseSlideNumbers(value));
    }
}
