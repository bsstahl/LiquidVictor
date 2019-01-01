using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Linq;
using System.Threading.Tasks;

namespace LiquidVictor.Output.RevealJs.Layout.Service
{
    public static class PartConventionBuilderExtensions
    {
        public static PartConventionBuilder UseParameterlessConstructor(this PartConventionBuilder builder)
        {
            return builder
                .SelectConstructor(c => c.Where(ci => !ci.GetParameters().Any()).Single());
        }
    }
}
