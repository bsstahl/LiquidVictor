using LiquidVictor.Enumerations;
using System;

namespace LiquidVictor.Exceptions;

public class SlideLayoutException : Exception
{
    public SlideLayoutException()
    { }

    public SlideLayoutException(Layout layout, string errorMessage)
        : base($"Layout='{layout}' - {errorMessage}")
    {
        this.Layout = layout;
    }

    public SlideLayoutException(string message) 
        : base(message)
    { }

    public SlideLayoutException(string message, Exception innerException) 
        : base(message, innerException)
    { }


    public Layout Layout { get; set; }

}
