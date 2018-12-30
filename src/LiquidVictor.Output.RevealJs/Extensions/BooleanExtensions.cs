using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Output.RevealJs.Extensions
{
    public static class BooleanExtensions
    {
        public static string AsInOut(this bool value)
        {
            if (value)
                return "in";
            else
                return "out";
        }
    }
}
