using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.RunTime.Values.Interfaces
{
    public interface IRunTimeValue
    {
        ValueType Type { get; }
    }
}
