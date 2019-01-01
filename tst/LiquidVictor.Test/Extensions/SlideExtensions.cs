using LiquidVictor.Entities;
using LiquidVictor.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TestHelperExtensions;

namespace LiquidVictor.Test.Extensions
{
    public static class SlideExtensions
    {
        public static Slide GetRandom(this Slide ignore)
        {
            return new SlideBuilder()
                .AssignId()
                .Notes(string.Empty.GetRandom())
                .Title(string.Empty.GetRandom())
                .Layout(Enumerations.Layout.Title.GetRandom())
                .TransitionIn(Enumerations.Transition.None.GetRandom())
                .TransitionOut(Enumerations.Transition.None.GetRandom())
                .Build();

                //ContentItems = new List<KeyValuePair<int, ContentItem>>()
        }
    }
}
