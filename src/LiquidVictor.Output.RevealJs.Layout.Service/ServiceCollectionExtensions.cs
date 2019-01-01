using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;

namespace LiquidVictor.Output.RevealJs.Layout.Service
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLayoutStrategy(this IServiceCollection services)
        {
            var conventions = new ConventionBuilder();

            conventions
                .ForTypesDerivedFrom<Interfaces.ILayoutStrategy>()
                .Export<Interfaces.ILayoutStrategy>()
                .UseParameterlessConstructor()
                .Shared();

            var configuration = new ContainerConfiguration()
                .WithAssembliesInRoot(conventions);

            Type result;
            using (var container = configuration.CreateContainer())
            {
                var strategyTypes = container.GetExports<Interfaces.ILayoutStrategy>();
                if (strategyTypes.Count() != 1)
                    throw new CompositionFailedException("There must be exactly 1 instance of an ILayoutStrategy in the root folder");
                result = strategyTypes.Single().GetType();
            }

            return services.AddTransient(typeof(Interfaces.ILayoutStrategy), result);
        }

    }
}
