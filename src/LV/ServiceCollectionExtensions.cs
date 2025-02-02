using LiquidVictor.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LV;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationBuilder(this IServiceCollection services, Configuration config)
    {
        switch (config.OutputEngineType.ToLower())
        {
            case "powerpoint":
            case "pptx":
            case "ppt":
                services.AddTransient<IPresentationBuilder>(c =>
                {
                    var builderOptions = new LiquidVictor.Output.Powerpoint.Entities.BuilderOptions()
                    {
                        BuildTitleSlide = config.BuildTitleSlide,
                        MakeSoloImagesFullScreen = config.MakeSoloImagesFullScreen
                    };
                    return new LiquidVictor.Output.Powerpoint.Generator.Engine(builderOptions);
                });
        break;
            case "reveal":
        case "revealjs":
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
        default:
            throw new NotSupportedException($"Invalid Presentation Builder '{config.OutputEngineType};");
        }

        return services;
    }

    public static IServiceCollection AddReadRepository(this IServiceCollection services, Configuration config)
    {
        switch (config.SourceRepoType.ToLower())
        {
            case "postgres":
            case "postgresql":
                services.AddTransient<ISlideDeckReadRepository, LiquidVictor.Data.Postgres.SlideDeckReadRepository>();
                break;
            case "yamlfile":
                services.AddTransient<ISlideDeckReadRepository>(c => new LiquidVictor.Data.YamlFile.SlideDeckReadRepository(config.SourceRepoPath));
                break;
            default:
                services.AddTransient<ISlideDeckReadRepository>(c => new LiquidVictor.Data.JsonFileSystem.SlideDeckReadRepository(config.SourceRepoPath));
                break;
        }
        return services;
    }

    public static IServiceCollection AddWriteRepository(this IServiceCollection services, Configuration config)
    {
        switch (config.SourceRepoType.ToLower())
        {
            case "postgres":
            case "postgresql":
                services.AddTransient<ISlideDeckWriteRepository>(c => new LiquidVictor.Data.Postgres.SlideDeckWriteRepository(config.SourceRepoPath));
                break;
            case "yamlfile":
                services.AddTransient<ISlideDeckWriteRepository>(c => new LiquidVictor.Data.YamlFile.SlideDeckWriteRepository(config.SourceRepoPath));
                break;
            default:
                services.AddTransient<ISlideDeckWriteRepository>(c => new LiquidVictor.Data.JsonFileSystem.SlideDeckWriteRepository(config.SourceRepoPath));
                break;
        }
        return services;
    }

    public static IServiceCollection AddStrategies(this IServiceCollection services)
    {
        return services
            .AddTransient<ITableOfContentsStrategy, LiquidVictor.Strategy.TableOfContents.Engine>();
    }
}
