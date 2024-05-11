using LiquidVictor.Business;
using LiquidVictor.Data.Postgres;
using LiquidVictor.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace LV;

class Program
{
    static void Main(string[] args)
    {
        const string defaultConfigPath = @"defaults.json";

        string executionFolder = Path.GetDirectoryName(AppContext.BaseDirectory) ?? throw new InvalidOperationException($"{nameof(AppContext.BaseDirectory)} is returning a null directory name");
        string fullConfigPath = Path.Combine(executionFolder, defaultConfigPath);

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

        var services = new ServiceCollection()
            .AddReadRepository(config)
            .AddWriteRepository(config)
            .AddPresentationBuilder(config)
            .AddCommandEngine()
            .AddStrategies()
            .BuildServiceProvider();

        // TODO: Restore
        command.Execute(services, config);
        //try
        //{
        //    engine.Execute();
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine($"Error: {ex.Message}");
        //}        
    }

}
