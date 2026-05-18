using LiquidVictor.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace LV;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationBuilder(this IServiceCollection services, Configuration config)
    {
        switch (config.OutputEngineType.ToUpperInvariant())
        {
            case "REVEAL":
            case "REVEALJS":
                services
                    .AddTransient<IPresentationBuilder>(c =>
                    {
                        var builderOptions = new LiquidVictor.Output.RevealJs.Entities.BuilderOptions()
                        {
                            BuildTitleSlide = config.BuildTitleSlide,
                            MakeSoloImagesFullScreen = config.MakeSoloImagesFullScreen
                        };
                        return new LiquidVictor.Output.RevealJs.Generator.Engine(config.TemplatePath, builderOptions);
                    });
                break;
            case "JUPYTER":
                services
                    .AddTransient<IPresentationBuilder>(c =>
                    {
                        var builderOptions = new LiquidVictor.Output.Jupyter.Entities.BuilderOptions()
                        {
                            BuildTitleSlide = config.BuildTitleSlide
                        };
                        return new LiquidVictor.Output.Jupyter.Generator.Engine(builderOptions);
                    });
                break;
            default:
                throw new NotSupportedException($"Invalid Presentation Builder '{config.OutputEngineType};");
        }

        return services;
    }

    public static IServiceCollection AddReadRepository(this IServiceCollection services, Configuration config)
    {
        switch (config.SourceRepoType.ToUpperInvariant())
        {
            case "POSTGRES":
            case "POSTGRESQL":
                services.AddTransient<ISlideDeckReadRepository, LiquidVictor.Data.Postgres.SlideDeckReadRepository>();
                break;
            case "YAMLFILE":
                services.AddTransient<ISlideDeckReadRepository>(c => new LiquidVictor.Data.YamlFile.SlideDeckReadRepository(config.SourceRepoPath));
                break;
            default:
                throw new NotSupportedException($"Invalid Source Repository Type '{config.SourceRepoType};");
        }
        return services;
    }

    public static IServiceCollection AddWriteRepository(this IServiceCollection services, Configuration config)
    {
        switch (config.SourceRepoType.ToUpperInvariant())
        {
            case "POSTGRES":
            case "POSTGRESQL":
                services.AddTransient<ISlideDeckWriteRepository>(c => new LiquidVictor.Data.Postgres.SlideDeckWriteRepository(config.SourceRepoPath));
                break;
            case "YAMLFILE":
                services.AddTransient<ISlideDeckWriteRepository>(c => new LiquidVictor.Data.YamlFile.SlideDeckWriteRepository(config.SourceRepoPath));
                break;
            default:
                throw new NotSupportedException($"Invalid Target Repository Type '{config.SourceRepoType};");
        }
        return services;
    }

    public static IServiceCollection AddStrategies(this IServiceCollection services)
    {
        return services
            .AddTransient<ITableOfContentsStrategy, LiquidVictor.Strategy.TableOfContents.Engine>();
    }
}
