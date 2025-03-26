using Newtonsoft.Json;
using System;
using VibeScript.RunTime.Values.Interfaces;

namespace VibeScript.RunTime.Values
{
    public class RuntimeValueJsonConverter : JsonConverter<IRunTimeValue>
    {
        public override IRunTimeValue ReadJson(JsonReader reader, Type objectType, IRunTimeValue existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Deserialization is not supported for runtime values");
        }

        public override void WriteJson(JsonWriter writer, IRunTimeValue value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(value.Type.ToString());

            writer.WritePropertyName("Value");
            switch (value)
            {
                case NumberValue num:
                    writer.WriteValue(num.Value);
                    break;
                case BooleanValue boolean:
                    writer.WriteValue(boolean.Value);
                    break;
                case NullValue:
                    writer.WriteNull();
                    break;
                case ObjectValue obj:
                    writer.WriteStartObject();
                    foreach (var prop in obj.Properties)
                    {
                        writer.WritePropertyName(prop.Key);
                        serializer.Serialize(writer, prop.Value);
                    }
                    writer.WriteEndObject();
                    break;
                case NativeFuncValue:
                    writer.WriteValue("[Native Function]");
                    break;
                case FuncValue:
                    writer.WriteValue("[Function]");
                    break;
                default:
                    writer.WriteNull();
                    break;
            }
            writer.WriteEndObject();
        }
    }
} 