using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace LiquidVictor.Data.YamlFile;

public class ChildIdYamlConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(ChildId);
    }


    public object ReadYaml(IParser parser, Type type)
    {
        var id = Guid.Parse(((Scalar)parser.Current).Value);
        parser.MoveNext();
        return new ChildId(id, string.Empty);
    }


    public void WriteYaml(IEmitter emitter, object value, Type type)
    {
        var childId = (ChildId)value;
        emitter.Emit(new Scalar(null, null, childId.Id.ToString(), ScalarStyle.Plain, true, false));
        if (!string.IsNullOrWhiteSpace(childId.Title))
            emitter.Emit(new Comment(childId.Title, true));
    }
}