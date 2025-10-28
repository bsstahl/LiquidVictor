namespace LiquidVictor.Data.YamlFile.Test;

public class SlideDeckReadRepository_GetSlideDeck_Should
{
    [Fact]
    [Trait("Category", "Integration")]
    [Trait("Status", "Incomplete")]
    public void ReturnTheSampleSlideDeck()
    {
        var repoPath = Path.GetFullPath(@".\TestRepo");
        var slideDeckId = Guid.Parse("64b458e1-be36-4f42-86b0-63f7a9ca1503");

        var repo = new SlideDeckReadRepository(repoPath);
        var result = repo.GetSlideDeck(slideDeckId);

        throw new NotImplementedException();
    }
}