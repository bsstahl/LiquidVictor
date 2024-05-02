namespace LiquidVictor.Test
{
    public class ContentItem_Clone_Should
    {
        [Theory]
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