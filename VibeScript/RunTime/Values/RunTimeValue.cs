
using VibeScript.RunTime.Environment;
using VibeScript.RunTime.Values.Interfaces;

namespace VibeScript.RunTime.Values
{
    public abstract class RunTimeValue : IRunTimeValue
    {
        public delegate RunTimeValue FunctionCall(RunTimeValue[] args, RuntimeEnvironment env);
        public abstract ValueType Type { get; }
    }
}
