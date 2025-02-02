using DocumentFormat.OpenXml.Packaging;
using LiquidVictor.Entities;

namespace LiquidVictor.Output.Powerpoint.Interfaces
{
    public interface ILayoutStrategy
    {
        void Layout(SlidePart slidePart, Slide slide);
    }
}
