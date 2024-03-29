﻿using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Output.RevealJs.Interfaces
{
    public interface ILayoutStrategy
    {
        string Layout(Slide slide, int zeroBasedIndex);
    }
}
