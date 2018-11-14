using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Output.RevealJs
{
    public static class SlideDeckExtensions
    {
        public static (int, int) GetPresentationSize(this SlideDeck deck)
        {
            int width, height;
            switch (deck.AspectRatio)
            {
                case Enumerations.AspectRatio.Widescreen:
                    width = 1920;
                    height = 1080;
                    break;
                case Enumerations.AspectRatio.Standard:
                    width = 1024;
                    height = 768;
                    break;
                default:
                    throw new NotSupportedException($"Invalid Aspect Ratio {deck.AspectRatio.ToString()}");
            }

            return (width, height);
        }
    }
}
