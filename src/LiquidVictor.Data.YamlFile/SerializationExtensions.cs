using YamlDotNet.Serialization;

namespace LiquidVictor.Data.YamlFile;

internal static class SerializationExtensions
{

    internal static string ParseId(this string yaml)
    {
        var deserializer = new DeserializerBuilder().Build();
        var yamlObject = deserializer.Deserialize<dynamic>(yaml);
        object id = string.Empty;
        _ = yamlObject.TryGetValue("Id", out id) || yamlObject.TryGetValue("id", out id);
        return id.ToString();
    }

}
