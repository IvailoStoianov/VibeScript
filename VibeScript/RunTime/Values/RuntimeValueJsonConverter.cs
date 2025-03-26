using Newtonsoft.Json;
using System;
using VibeScript.RunTime.Values.Interfaces;

namespace VibeScript.RunTime.Values
{
    /// <summary>
    /// Custom JSON converter for runtime values, handles circular references and different value types.
    /// </summary>
    public class RuntimeValueJsonConverter : JsonConverter<IRunTimeValue>
    {
        public override IRunTimeValue ReadJson(JsonReader reader, Type objectType, IRunTimeValue existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Deserialization is not supported for runtime values");
        }

        public override void WriteJson(JsonWriter writer, IRunTimeValue value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

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
                    WriteObjectValue(writer, obj, serializer);
                    break;
                case NativeFuncValue:
                    writer.WriteValue("[Native Function]");
                    break;
                case FuncValue func:
                    WriteFunctionValue(writer, func, serializer);
                    break;
                default:
                    writer.WriteNull();
                    break;
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Writes an object value to JSON, handling its properties.
        /// </summary>
        private void WriteObjectValue(JsonWriter writer, ObjectValue obj, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            foreach (var prop in obj.Properties)
            {
                writer.WritePropertyName(prop.Key);
                serializer.Serialize(writer, prop.Value);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Writes a function value to JSON, showing its name and parameters.
        /// </summary>
        private void WriteFunctionValue(JsonWriter writer, FuncValue func, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Name");
            writer.WriteValue(func.Name);
            
            writer.WritePropertyName("Parameters");
            writer.WriteStartArray();
            foreach (var param in func.Parameters)
            {
                writer.WriteValue(param);
            }
            writer.WriteEndArray();
            
            writer.WriteEndObject();
        }
    }
} 