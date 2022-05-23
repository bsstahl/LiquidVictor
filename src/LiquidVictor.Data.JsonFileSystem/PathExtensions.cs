using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Data.JsonFileSystem
{
    internal static class PathExtensions
    {
        internal static string FindFileWithId(this string path, Guid Id)
        {
            string result = string.Empty;
            var jsonFiles = System.IO.Directory.EnumerateFiles(path, "*.json");
            foreach (var jsonFile in jsonFiles)
            {
                if (string.IsNullOrEmpty(result))
                {
                    var json = System.IO.File.ReadAllText(jsonFile);
                    var item = Newtonsoft.Json.Linq.JObject.Parse(json);
                    if (item.Value<string>("Id").Equals(Id.ToString()))
                        result = jsonFile;
                }
            }

            return result;
        }
    }
}
