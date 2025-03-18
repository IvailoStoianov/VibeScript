

using VibeScript.RunTime.Values.Interfaces;

namespace VibeScript.RunTime.Values
{
    public class ObjectValue : RunTimeValue
    {
        public ObjectValue(Dictionary<string, IRunTimeValue> properties) 
        {
            this.Properties = properties;
        }
        public override ValueType Type => ValueType.Object;
        public Dictionary<string, IRunTimeValue> Properties { get; set; } = new();
    }
}
