using System;
using LiquidVictor.Enumerations;

namespace LiquidVictor.Output.RevealJs.Extensions
{
    public static class LayoutExtensions
    {
        public static string AsComment(this Layout value)
        {
            return value.ToString().AsComment("Layout");
        }
    }
}
