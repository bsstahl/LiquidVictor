using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Data.JsonFileSystem
{
    internal static class PathExtensions
    {
        internal static string FindFileWithId(this string path, Guid id)
        {
            string result = string.Empty;
            var jsonFiles = System.IO.Directory.EnumerateFiles(path, "*.json");
            foreach (var jsonFile in jsonFiles)
            {
                string currentFileId = System.IO.File.ReadAllText(jsonFile).ParseId();
                if (currentFileId.Equals(id.ToString()))
                {
                    result = !string.IsNullOrEmpty(result) 
                        ? throw new InvalidOperationException($"Multiple Slide Decks have Id={id}") 
                        : jsonFile;
                }
            }

            return result;
        }

        internal static IEnumerable<Guid> GetFileIds(this string path)
        {
            var result = new List<Guid>();

            var jsonFiles = System.IO.Directory.EnumerateFiles(path, "*.json");
            foreach (var jsonFile in jsonFiles)
            {
                string id = System.IO.File.ReadAllText(jsonFile).ParseId();
                result.Add(Guid.Parse(id));
            }

            return result;
        }
    }
}
