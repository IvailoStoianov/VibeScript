using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibeScript.RunTime.Values.Interfaces;

namespace VibeScript.RunTime.Values
{
    public abstract class RunTimeValue : IRunTimeValue
    {
        public abstract ValueType Type { get; }
    }
}
