#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;

namespace Atheneum;

/// <summary>
/// Validate required Properties within Class
/// </summary>
/// <remarks>
/// Content taken from https://github.com/aaubry/YamlDotNet/issues/202
/// </remarks>
public class ValidatingDeserializer : INodeDeserializer
{
    private readonly INodeDeserializer _nodeDeserializer;

    public ValidatingDeserializer(INodeDeserializer nodeDeserializer)
    {
        _nodeDeserializer = nodeDeserializer;
    }

    public bool Deserialize(IParser parser, Type expectedType,
        Func<IParser, Type, object?> nestedObjectDeserializer, out object? value)
    {
        if (!_nodeDeserializer.Deserialize(parser, expectedType, nestedObjectDeserializer, out value) ||
            value == null)
        {
            return false;
        }

        var context = new ValidationContext(value, null, null);

        try
        {
            Validator.ValidateObject(value, context, true);
        }
        catch (ValidationException e)
        {
            if (parser.Current == null)
            {
                throw;
            }

            throw new YamlException(parser.Current.Start, parser.Current.End, e.Message);
        }

        return true;
    }
}
public static class YamlDotNetExtensions
{
    public static DeserializerBuilder WithRequiredPropertyValidation(this DeserializerBuilder builder)
    {
        return builder
            .WithNodeDeserializer(inner => new ValidatingDeserializer(inner),
                s => s.InsteadOf<ObjectNodeDeserializer>());
    }
}
public class YamlStringEnumConverter : IYamlTypeConverter
{
    // Only run if this is an Enum
    public bool Accepts(Type type) => type.IsEnum;

    public object ReadYaml(IParser parser, Type type)
    {
        var parsedEnum = parser.Consume<Scalar>();
        string normalizeEnum = parsedEnum.Value.Replace(" ", "").ToLower();

        try
        {
            return Enum.Parse(type, normalizeEnum, true);
        }
        catch
        {
            throw new YamlException(parsedEnum.Start, parsedEnum.End, $"Value '{normalizeEnum}' not found in enum '{type.Name}'");

        }
    }

    public void WriteYaml(IEmitter emitter, object value, Type type)
    {
        var enumMember = type.GetMember(value.ToString()).FirstOrDefault();
        var yamlValue = enumMember?.GetCustomAttributes<EnumMemberAttribute>(true).Select(ema => ema.Value).FirstOrDefault() ?? value.ToString();
        emitter.Emit(new Scalar(yamlValue));
    }
    private static bool NodeIsNull(NodeEvent nodeEvent)
    {
        // http://yaml.org/type/null.html

        if (nodeEvent.Tag == "tag:yaml.org,2002:null")
        {
            return true;
        }

        if (nodeEvent is Scalar scalar && scalar.Style == ScalarStyle.Plain)
        {
            var value = scalar.Value;
            return value is "" or "~" or "null" or "Null" or "NULL";
        }

        return false;
    }
}
//public class YamlStringListConverter : IYamlTypeConverter
//{
//    // Only run if this is a List of Strings
//    public bool Accepts(Type type) => type is IList<string>;

//    public object ReadYaml(IParser parser, Type type)
//    {
//        var deserializer = ...;
//        if (parser.TryConsume<SequenceStart>(out _))
//        {
//            var items = new List<string>();
//            // read until end of sequence
//            while (!parser.TryConsume<SequenceEnd>(out _))
//            {
//                // skip comments
//                if (parser.TryConsume<Comment>(out _))
//                {
//                    continue;
//                }

//                var item = deserializer.Deserialize<string>(parser);
//                items.Add(item);
//            }

//            return CreateReturnValue(type, items);
//        }

//        var singleValue = deserializer.Deserialize<string>(parser);
//        if (singleValue == null)
//        {
//            return null;
//        }

//        return CreateReturnValue(type, new[] { singleValue })
//    }

//    public void WriteYaml(IEmitter emitter, object value, Type type)
//    {
//        var enumMember = type.GetMember(value.ToString()).FirstOrDefault();
//        var yamlValue = enumMember?.GetCustomAttributes<EnumMemberAttribute>(true).Select(ema => ema.Value).FirstOrDefault() ?? value.ToString();
//        emitter.Emit(new Scalar(yamlValue));
//    }
//    private static bool NodeIsNull(NodeEvent nodeEvent)
//    {
//        // http://yaml.org/type/null.html

//        if (nodeEvent.Tag == "tag:yaml.org,2002:null")
//        {
//            return true;
//        }

//        if (nodeEvent is Scalar scalar && scalar.Style == ScalarStyle.Plain)
//        {
//            var value = scalar.Value;
//            return value is "" or "~" or "null" or "Null" or "NULL";
//        }

//        return false;
//    }
//}