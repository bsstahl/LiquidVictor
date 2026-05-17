using System;
using System.Data;
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
        ArgumentNullException.ThrowIfNull(parser);
        ArgumentNullException.ThrowIfNull(parser.Current);

        var currentScalar = parser.Current as Scalar;
        var idString = currentScalar?.Value ?? Guid.Empty.ToString();
        var id = Guid.Parse(idString);
        parser.MoveNext();
        return new ChildId(id, string.Empty);
    }


    public void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        ArgumentNullException.ThrowIfNull(emitter);

        var childId = value as ChildId;
        var thisId = childId?.Id.ToString() ?? string.Empty;
        var thisTitle = childId?.Title ?? string.Empty;
        emitter.Emit(new Scalar(null, null, thisId, ScalarStyle.Plain, true, false));
        if (!string.IsNullOrWhiteSpace(thisTitle))
            emitter.Emit(new Comment(thisTitle, true));
    }
}