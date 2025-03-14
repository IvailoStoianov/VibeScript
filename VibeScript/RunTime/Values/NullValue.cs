using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.RunTime.Values
{
    public class NullValue : RunTimeValue
    {
        public override ValueType Type => ValueType.Null;
        public object? Value => null;
    }
}
