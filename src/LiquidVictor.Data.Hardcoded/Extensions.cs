using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Data.Hardcoded
{
    public static class Extensions
    {
        public static void Add(this List<KeyValuePair<int, Entities.Slide>> list, int index, Entities.Slide slide)
        {
            list.Add(new KeyValuePair<int, Entities.Slide>(index, slide));
        }
    }
}
