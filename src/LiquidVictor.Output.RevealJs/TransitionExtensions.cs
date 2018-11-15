using LiquidVictor.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Output.RevealJs
{
    public static class TransitionExtensions
    {
        public static string GetClass(this Transition transition, bool isInTransition)
        {
            string result = string.Empty;
            if (transition != Transition.PresentationDefault)
                result = $"{transition.ToString().ToLower()}-{isInTransition.AsInOut()}";
            return result;
        }
    }
}
