using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Data.YamlFile;

internal static class PathExtensions
{
    internal static string FindFileWithId(this string path, Guid id)
    {
        string result = string.Empty;
        var yamlFiles = System.IO.Directory.EnumerateFiles(path, "*.yaml");
        foreach (var yamlFile in yamlFiles)
        {
            string currentFileId = System.IO.File.ReadAllText(yamlFile).ParseId();
            if (currentFileId.Equals(id.ToString()))
            {
                result = !string.IsNullOrEmpty(result) 
                    ? throw new InvalidOperationException($"Multiple Slide Decks have Id={id}") 
                    : yamlFile;
            }
        }

        return result;
    }

    internal static IEnumerable<Guid> GetFileIds(this string path)
    {
        var result = new List<Guid>();

        var files = System.IO.Directory.EnumerateFiles(path, "*.yaml");
        foreach (var file in files)
        {
            var textId = string.Empty;
            Guid id = Guid.Empty;
            var fileName = System.IO.Path.GetFileNameWithoutExtension(file);
            if (!Guid.TryParse(fileName, out id))
            {
                textId = System.IO.File.ReadAllText(file).ParseId();
                _ = Guid.TryParse(textId, out id);
            }

            if (id.Equals(Guid.Empty))
                Console.WriteLine($"Unable to parse Id from '{file}'");
            else
                result.Add(id);
        }

        return result;
    }
}
