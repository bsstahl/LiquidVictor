using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Output.RevealJs
{
    internal interface ILayoutStrategy
    {
        string Layout(SlideDeck deck, Slide slide);
    }
}
