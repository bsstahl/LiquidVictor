using System;
using System.Collections.Generic;
using System.Text;
using TestHelperExtensions;

namespace LiquidVictor.Test.Extensions
{
    public static class TransitionExtensions
    {
        public static Enumerations.Transition GetRandom(this Enumerations.Transition ignore)
        {
            return (Enumerations.Transition)(Enum.GetValues(typeof(Enumerations.Transition)).Length).GetRandom();
        }
    }
}
