using LiquidVictor.Builders;
using LiquidVictor.Data.Test.Extensions;
using TestHelperExtensions;

namespace LiquidVictor.Data.JsonFileSystem
{
    public class SlideDeckReadRepository_FindDuplicateIds_Should
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ReturnTheDuplicateSlideId_WhenTwoSlidesInADeckShareAnId()
        {
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
                                .UseRandomValues()))
                        .Add(new SlideBuilder()
                            .UseRandomValues()
                            .ContentItems(new ContentItemBuilder()
                                    .UseRandomValues())
                            .ContentItems(new ContentItemBuilder()
                                    .UseRandomValues()))
                        .Add(new SlideBuilder()
                            .UseRandomValues()
                            .Id(slideId)
                            .ContentItems(new ContentItemBuilder()
                                .UseRandomValues())
                            .ContentItems(new ContentItemBuilder()
                            .UseRandomValues()))));

            var results = SlideDeckReadRepository.FindDuplicateIds(slideDecks.Build());

            Assert.Single(results.SlideIds);
            Assert.Contains(slideId, results.SlideIds);
            Assert.Empty(results.ContentItemIds);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReturnTheDuplicateContentItemId_WhenTwoContentItemsInADeckShareAnId()
        {
            var contentItemId = Guid.NewGuid();

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
                            .ContentItems(new ContentItemBuilder()
                                .UseRandomValues())
                            .ContentItems(new ContentItemBuilder()
                            .UseRandomValues()))));

            var results = SlideDeckReadRepository.FindDuplicateIds(slideDecks.Build());

            Assert.Single(results.ContentItemIds);
            Assert.Contains(contentItemId, results.ContentItemIds);
            Assert.Empty(results.SlideIds);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReturnNoDuplicateSlideDeckIds_WhenOnlyOneSlideDeckIsProvided()
        {
            var slideDecks = new SlideDecksBuilder()
                .Add(new SlideDeckBuilder()
                    .Id(Guid.NewGuid())
                    .Title(string.Empty.GetRandom())
                    .SubTitle(string.Empty.GetRandom())
                    .Presenter(string.Empty.GetRandom())
                    .PrintLinkText(string.Empty.GetRandom())
                    .Slides(new SlidesBuilder()
                        .Add(new SlideBuilder().UseRandomValues())
                        .Add(new SlideBuilder().UseRandomValues())));

            var results = SlideDeckReadRepository.FindDuplicateIds(slideDecks.Build());

            Assert.Empty(results.SlideDeckIds);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReturnNoDuplicates_WhenAllIdsAreUnique()
        {
            var slideDecks = new SlideDecksBuilder()
                .Add(new SlideDeckBuilder()
                    .Id(Guid.NewGuid())
                    .Title(string.Empty.GetRandom())
                    .SubTitle(string.Empty.GetRandom())
                    .Presenter(string.Empty.GetRandom())
                    .PrintLinkText(string.Empty.GetRandom())
                    .Slides(new SlidesBuilder()
                        .Add(new SlideBuilder().UseRandomValues())
                        .Add(new SlideBuilder().UseRandomValues())));

            var results = SlideDeckReadRepository.FindDuplicateIds(slideDecks.Build());

            Assert.Empty(results.SlideDeckIds);
            Assert.Empty(results.SlideIds);
            Assert.Empty(results.ContentItemIds);
        }
    }
}
