using Microsoft.Extensions.DependencyInjection;
using System;

namespace LiquidVictor.Business;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandEngine(this IServiceCollection services)
    {
        return services.AddTransient<Interfaces.ICommandEngine, Engine>();
    }
}
