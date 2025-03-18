using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibeScript.RunTime.Values.Interfaces;

namespace VibeScript.RunTime.Values
{
    public class BooleanValue : RunTimeValue
    {
        //Default to true
        public BooleanValue(bool value = true) 
        {
            Value = value;
        }
        public override ValueType Type => ValueType.Boolean;
        public bool Value { get; set; }
    }
}
