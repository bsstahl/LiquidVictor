using System;
using System.Collections.Generic;
using System.Linq;

namespace LiquidVictor.Entities;

internal class IncludeBlockCollection: List<IncludeBlock>
{
    internal IncludeBlockCollection()
    { }

    internal IncludeBlockCollection(IOrderedEnumerable<IncludeBlock> includes)
    {
        this.AddRange(includes);
    }

    internal void Add(Slide slide)
    {
        throw new NotImplementedException(); 
    }

    internal void Add(IOrderedEnumerable<Slide> slides)
    {
        throw new NotImplementedException();
    }
}
