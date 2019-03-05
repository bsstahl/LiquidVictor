using LiquidVictor.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Output.RevealJs.Extensions
{
    public static class TransitionExtensions
    {
        public static string GetClass(this Transition transition, bool isInTransition, Transition presentationDefault)
        {
            var transitionToUse = (transition == Transition.PresentationDefault) ? presentationDefault : transition;
            return $"{transitionToUse.GetTransitionBaseName()}-{isInTransition.AsInOut()}";
        }

        public static string GetTransitionBaseName(this Transition transition)
        {
            string result = "slide";

            switch (transition)
            {
                case Transition.None:
                case Transition.Fade:
                case Transition.Slide:
                    result = transition.ToString().ToLower();
                    break;
                case Transition.Fancy:
                    result = "convex";
                    break;
                default:
                    break;
            }

            return result;

        }
    }
}
