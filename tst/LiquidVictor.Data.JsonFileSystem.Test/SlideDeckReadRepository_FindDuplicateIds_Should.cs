using LiquidVictor.Builders;
using LiquidVictor.Data.Test.Extensions;
using TestHelperExtensions;

namespace LiquidVictor.Data.JsonFileSystem
{
    public class SlideDeckReadRepository_FindDuplicateIds_Should
    {
        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Status", "Incomplete")]
        public void DoAThing()
        {
            var contentItemId = Guid.NewGuid();
            var slideId = Guid.NewGuid();

            var slideDecks = new SlideDecksBuilder()
                .Add(new SlideDeckBuilder()
                    .Id(Guid.NewGuid())
                    .Title(string.Empty.GetRandom())
                    .SubTitle(string.Empty.GetRandom())
                    .Presenter(string.Empty.GetRandom())
                    .PrintLinkText(string.Empty.GetRandom())
                    .Slides(new SlidesBuilder()
                        .Add(new SlideBuilder()
                            .UseRandomValues()
                            .Id(slideId)
                            .ContentItems(new ContentItemBuilder()
                                .UseRandomValues()
                                .Id(contentItemId)))
                        .Add(new SlideBuilder()
                            .UseRandomValues()
                            .ContentItems(new ContentItemBuilder()
                                    .UseRandomValues())
                            .ContentItems(new ContentItemBuilder()
                                    .UseRandomValues()
                                    .Id(contentItemId)))
                        .Add(new SlideBuilder()
                            .UseRandomValues()
                            .Id(slideId)
                            .ContentItems(new ContentItemBuilder()
                                .UseRandomValues())
                            .ContentItems(new ContentItemBuilder()
                            .UseRandomValues()))));

            var results = SlideDeckReadRepository.FindDuplicateIds(slideDecks.Build());

            Assert.Fail("Complete this test");

        }
    }
}