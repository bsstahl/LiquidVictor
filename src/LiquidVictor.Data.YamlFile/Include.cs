using YamlDotNet.Serialization;

namespace LiquidVictor.Data.YamlFile;

internal class Include
{
    public string Id { get; set; } = string.Empty;
    public string IncludeType { get; set; } = Enumerations.IncludeType.Slide.ToString();
}
