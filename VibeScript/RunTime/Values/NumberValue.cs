using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.RunTime.Values
{
    public class NumberValue : RunTimeValue
    {
        public NumberValue(float value) 
        { 
            Value = value;
        }
        public override ValueType Type => ValueType.Number;
        public float Value { get; set; }
    }
}
