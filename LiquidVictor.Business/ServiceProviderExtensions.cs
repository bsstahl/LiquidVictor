using LiquidVictor.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Business
{
    public static class ServiceProviderExtensions
    {
        public static ISlideDeckReadRepository GetReadRepo(this IServiceProvider services)
            => services.GetRequiredService<ISlideDeckReadRepository>();

        public static ISlideDeckWriteRepository GetWriteRepo(this IServiceProvider services)
            => services.GetRequiredService<ISlideDeckWriteRepository>();

        public static IPresentationBuilder GetPresentationBuilder(this IServiceProvider services)
            => services.GetRequiredService<IPresentationBuilder>();

        public static ITableOfContentsStrategy GetTocStrategy(this IServiceProvider services)
            => services.GetRequiredService<ITableOfContentsStrategy>();
    }
}
