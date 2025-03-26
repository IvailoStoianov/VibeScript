using VibeScript.RunTime.Values;
using VibeScript.RunTime.Values.Interfaces;
using ValueType = VibeScript.RunTime.Values.ValueType;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VibeScript.RunTime.Environment
{
    /// <summary>
    /// Represents an execution environment that holds variables and their values.
    /// Supports a parent-child relationship for scope management.
    /// </summary>
    public class RuntimeEnvironment
    {
        private readonly RuntimeEnvironment? _parent;
        private readonly Dictionary<string, IRunTimeValue> _variables;
        //Collection of all constants
        private readonly HashSet<string> _lockedIns;
        
        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter> { new RuntimeValueJsonConverter() }
        };
        
        /// <summary>
        /// Creates a new runtime environment with an optional parent environment.
        /// </summary>
        public RuntimeEnvironment(RuntimeEnvironment? parent = null) 
        {
            _parent = parent;
            _variables = new Dictionary<string, IRunTimeValue>();
            _lockedIns = new HashSet<string>();
        }

        /// <summary>
        /// Declares a new variable in the current environment.
        /// </summary>
        /// <param name="varName">The name of the variable.</param>
        /// <param name="value">The value to assign to the variable.</param>
        /// <param name="isLockedIn">Whether the variable is a constant (cannot be reassigned).</param>
        /// <returns>The assigned value.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the variable already exists.</exception>
        public IRunTimeValue DeclareVar(string varName, IRunTimeValue value, bool isLockedIn)
        {
            if (_variables.ContainsKey(varName))
            {
                throw new InvalidOperationException($"Cannot declare variable '{varName}' as it's already defined.");
            }
            
            if (isLockedIn)
            {
                _lockedIns.Add(varName);
            }
            
            _variables[varName] = value;
            return value;
        }

        /// <summary>
        /// Assigns a new value to an existing variable.
        /// </summary>
        /// <param name="varName">The name of the variable.</param>
        /// <param name="value">The new value to assign.</param>
        /// <returns>The assigned value.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the variable is constant or doesn't exist.</exception>
        public IRunTimeValue AssignVar(string varName, IRunTimeValue value)
        {
            RuntimeEnvironment env = Resolve(varName);
            
            if (env._lockedIns.Contains(varName))
            {
                throw new InvalidOperationException($"Cannot reassign to variable '{varName}' as it was declared as a constant (lockedIn).");
            }
            
            env._variables[varName] = value;
            return value;
        }

        /// <summary>
        /// Retrieves the value of a variable by name.
        /// </summary>
        /// <param name="varName">The name of the variable to look up.</param>
        /// <returns>The value of the variable.</returns>
        public IRunTimeValue LookUpVar(string varName)
        {
            RuntimeEnvironment env = Resolve(varName);
            return env._variables[varName];
        }

        /// <summary>
        /// Resolves which environment (current or ancestor) contains the variable.
        /// </summary>
        /// <param name="varName">The name of the variable to resolve.</param>
        /// <returns>The environment containing the variable.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the variable doesn't exist in any ancestor environment.</exception>
        public RuntimeEnvironment Resolve(string varName) 
        {
            if (_variables.ContainsKey(varName))
            {
                return this;
            }
            
            if (_parent == null)
            {
                throw new KeyNotFoundException($"Cannot resolve variable '{varName}' as it does not exist in any scope.");
            }
            
            return _parent.Resolve(varName);
        }

        /// <summary>
        /// Creates a global environment with built-in constants and functions.
        /// </summary>
        /// <returns>A new environment with global values.</returns>
        public RuntimeEnvironment CreateGlobalEnv()
        {
            RuntimeEnvironment env = new RuntimeEnvironment();

            // Define built-in constants
            env.DeclareVar("true", new BooleanValue(true), true);
            env.DeclareVar("false", new BooleanValue(false), true);
            env.DeclareVar("nah", new NullValue(), true);

            // Define the vibe() print function
            env.DeclareVar("vibe", new NativeFuncValue((args, scope) => PrintFunction(args, scope)), true);

            return env;
        }

        /// <summary>
        /// Implementation of the vibe() print function.
        /// </summary>
        private static NullValue PrintFunction(IEnumerable<RunTimeValue> args, RuntimeEnvironment scope)
        {
            if (args == null || !args.Any())
            {
                Console.WriteLine();
                return new NullValue();
            }

            Console.WriteLine(string.Join(" ", args.Select(FormatArgument)));
            return new NullValue();
        }

        /// <summary>
        /// Formats a runtime value for display.
        /// </summary>
        private static string FormatArgument(IRunTimeValue arg)
        {
            return arg.Type switch
            {
                ValueType.Null => "null",
                ValueType.Number => ((NumberValue)arg).Value.ToString(),
                ValueType.Boolean => ((BooleanValue)arg).Value ? "true" : "false",
                ValueType.Object => arg.ToString(),
                ValueType.NativeFunc => "[Native Function]",
                ValueType.Func => "[Function]",
                _ => "Unknown"
            };
        }
    }   
}
