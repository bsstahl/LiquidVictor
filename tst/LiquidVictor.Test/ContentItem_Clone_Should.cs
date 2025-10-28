namespace LiquidVictor.Test
{
    public class ContentItem_Clone_Should
    {
        [Theory]
        [Trait("Category", "Unit")]
        [InlineData(true)]
        [InlineData(false)]
        public void ReturnANewContentItemFromAMinimalSource(bool createNewId)
        {
            var source = new Entities.ContentItem();
            var target = source.Clone(createNewId);
            Assert.NotNull(target);
        }
    }
}