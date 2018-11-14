using LiquidVictor.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Exceptions
{
    public class SlideLayoutException : Exception
    {
        public Layout Layout { get; set; }

        public SlideLayoutException(Layout layout, string errorMessage)
            :base($"Layout='{layout.ToString()}' - {errorMessage}")
        {
            this.Layout = layout;
        }
    }
}
