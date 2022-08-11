using LiquidVictor.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LV
{
    class Program
    {
        static void Main(string[] args)
        {
            const string defaultConfigPath = @"defaults.json";

            string executionFolder = System.IO.Path.GetDirectoryName(AppContext.BaseDirectory);
            string fullConfigPath = System.IO.Path.Combine(executionFolder, defaultConfigPath);

            Command command;
            Configuration config;
            if (System.IO.File.Exists(fullConfigPath))
            {
                var defaults = new ConfigurationBuilder()
                    .AddJsonFile(defaultConfigPath, false)
                    .Build();
                (command, config) = args.Parse(defaults);
            }
            else
                (command, config) = args.Parse();

            var readRepo = GetReadRepository(config);
            var writeRepo = GetWriteRepository(config);
            var engine = GetEngine(config);

            try
            {
                command.Execute(config, readRepo, writeRepo, engine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }        
        }

        private static IPresentationBuilder GetEngine(Configuration config)
        {
            IPresentationBuilder result = null;
            switch (config.OutputEngineType.ToLower())
            {
                case "reveal":
                case "revealjs":
                    var builderOptions = new LiquidVictor.Output.RevealJs.Entities.BuilderOptions()
                    {
                        BuildTitleSlide = config.BuildTitleSlide,
                        MakeSoloImagesFullScreen = config.MakeSoloImagesFullScreen
                    };
                    result = new LiquidVictor.Output.RevealJs.Generator.Engine(config.TemplatePath, builderOptions);
                    break;
                default:
                    throw new NotSupportedException($"Invalid Presentation Builder '{config.OutputEngineType};");
            }

            return result;
        }

        private static ISlideDeckReadRepository GetReadRepository(Configuration config)
        {
            ISlideDeckReadRepository result = null;
            switch (config.SourceRepoType.ToLower())
            {
                case "postgres":
                case "postgresql":
                    result = new LiquidVictor.Data.Postgres.SlideDeckReadRepository();
                    break;
                default:
                    result = new LiquidVictor.Data.JsonFileSystem.SlideDeckReadRepository(config.SourceRepoPath);
                    break;
            }

            return result;
        }

        private static ISlideDeckWriteRepository GetWriteRepository(Configuration config)
        {
            ISlideDeckWriteRepository result = null;
            switch (config.SourceRepoType.ToLower())
            {
                case "postgres":
                case "postgresql":
                    result = new LiquidVictor.Data.Postgres.SlideDeckWriteRepository(config.SourceRepoPath);
                    break;
                default:
                    result = new LiquidVictor.Data.JsonFileSystem.SlideDeckWriteRepository(config.SourceRepoPath);
                    break;
            }

            return result;
        }

    }
}
