using VibeScript.RunTime.Values;
using VibeScript.RunTime.Values.Interfaces;
using ValueType = VibeScript.RunTime.Values.ValueType;

namespace VibeScript.RunTime.Environment
{
    public class RuntimeEnvironment
    {
        private RuntimeEnvironment? _parent;
        private Dictionary<string, IRunTimeValue> _variables;
        //Collection of all constants
        private HashSet<string> _lockedIns;
        
        public RuntimeEnvironment(RuntimeEnvironment? parent = null) 
        {
            this._parent = parent;
            this._variables = new Dictionary<string, IRunTimeValue>();
            this._lockedIns = new HashSet<string>();
        }

        public IRunTimeValue DeclareVar(string varName, IRunTimeValue value, bool isLockedIn)
        {
            if (_variables.ContainsKey(varName))
            {
                throw new Exception($"Cannot declare variable {varName} as it's already defined.");
            }
            if (isLockedIn)
            {
                _lockedIns.Add(varName);
            }
            this._variables[varName] = value;
            return value;
        }

        public IRunTimeValue AssignVar(string varName, IRunTimeValue value)
        {
            RuntimeEnvironment env = this.Resolve(varName);
            //Cannot assign to a constant
            if (env._lockedIns.Contains(varName))
            {
                throw new InvalidOperationException($"Cannot reasing to variable {varName} as it was declared a lockedIn.");
            }
            env._variables[varName] = value;
            return value;
        }
        public IRunTimeValue LookUpVar(string varName)
        {
            RuntimeEnvironment env = this.Resolve(varName);
            return env._variables[varName];
        }
        public RuntimeEnvironment Resolve(string varName) 
        {
            if (_variables.ContainsKey(varName))
            {
                return this;
            }
            if (_parent == null)
            {
                throw new Exception($"Cannot resolve {varName} as it does not exist.");
            }
            return this._parent.Resolve(varName);
        }

        public RuntimeEnvironment CreateGlobalEnv()
        {
            RuntimeEnvironment env = new RuntimeEnvironment();

            env.DeclareVar("true", new BooleanValue(true), true);
            env.DeclareVar("false", new BooleanValue(false), true);
            env.DeclareVar("nah", new NullValue(), true);

            // Define a native builtin method
            env.DeclareVar("vibe", new NativeFuncValue((args, scope) =>
            {
                //TODO: Make this a saperate function and make it better
                if (args == null || args.Count() == 0)
                {
                    // If no arguments, print an empty line
                    Console.WriteLine();
                    return new NullValue();
                }

                // Otherwise, iterate through the arguments and print them
                Console.WriteLine(string.Join(" ", args.Select(a => FormatArgument(a))));

                return new NullValue();
            }), true);

            return env;
        }

        private static string FormatArgument(IRunTimeValue arg)
        {
            switch (arg.Type)
            {
                case ValueType.Null:
                    return "null";
                case ValueType.Number:
                    return ((NumberValue)arg).Value.ToString();
                case ValueType.Boolean:
                    return ((BooleanValue)arg).Value ? "true" : "false";
                case ValueType.Object:
                    return arg.ToString();  // Customize this if necessary, to show more meaningful object info
                case ValueType.NativeFunc:
                    return "[Native Function]"; // You can customize this to print function details
                default:
                    return "Unknown";
            }
        }
    }   
}
