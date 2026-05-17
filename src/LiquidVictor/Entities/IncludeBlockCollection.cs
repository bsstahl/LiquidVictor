namespace LiquidVictor.Entities;

sealed internal class IncludeBlockCollection: List<IncludeBlock>
{
    internal IncludeBlockCollection()
    { }

    internal IncludeBlockCollection(IOrderedEnumerable<IncludeBlock> includes)
    {
        this.AddRange(includes);
    }

    internal void Add(Slide slide)
    {
        this.Add(new IncludeBlock(slide));
    }

    internal void Add(IOrderedEnumerable<Slide> slides)
    {
        this.Add(new IncludeBlock(slides));
    }
}
