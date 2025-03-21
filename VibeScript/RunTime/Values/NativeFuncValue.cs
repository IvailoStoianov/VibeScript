using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibeScript.RunTime.Values.Interfaces;
using static VibeScript.RunTime.Values.RunTimeValue;

namespace VibeScript.RunTime.Values
{
    public class NativeFuncValue : RunTimeValue
    {
        public NativeFuncValue(FunctionCall call)
        {
            Call = call;
        }

        public override ValueType Type => ValueType.NativeFunc;

        public FunctionCall Call { get; }
    }
}
