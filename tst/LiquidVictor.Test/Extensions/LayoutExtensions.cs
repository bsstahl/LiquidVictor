using System;
using System.Collections.Generic;
using System.Text;
using TestHelperExtensions;

namespace LiquidVictor.Test.Extensions
{
    public static class LayoutExtensions
    {
        public static Enumerations.Layout GetRandom(this Enumerations.Layout ignore)
        {
            return (Enumerations.Layout)(Enum.GetValues(typeof(Enumerations.Layout)).Length).GetRandom();
        }
    }
}
