using System;

namespace LiquidVictor.Data.JsonFileSystem
{
    internal static class SerializationExtensions
    {
        internal static void SerializeTo<T>(this T ci, string contentItemPath)
        {
            var textWriter = System.IO.File.CreateText(contentItemPath);
            var jsonWriter = new Newtonsoft.Json.JsonTextWriter(textWriter)
            {
                Indentation = 4,
            };

            var serializerSettings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            };
            var serializer = Newtonsoft.Json.JsonSerializer.Create(serializerSettings);

            serializer.Serialize(jsonWriter, ci);
            jsonWriter.Flush();
        }

    }
}
