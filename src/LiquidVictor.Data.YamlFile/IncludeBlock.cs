using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LiquidVictor.Data.YamlFile;

internal class IncludeBlock
{
    public string Id { get; set; } = string.Empty;
    public List<string> SlideIds { get; set; } = [];

    public static IncludeBlock Parse(string id, string yaml)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance) // matches YAML keys
            //.WithTypeConverter(new ChildIdYamlConverter())
            .Build();

        var result = deserializer.Deserialize<IncludeBlock>(yaml);
        result.Id = id;
    
        return result;
    }

}
