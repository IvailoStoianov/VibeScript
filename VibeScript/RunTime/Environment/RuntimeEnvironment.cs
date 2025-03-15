using VibeScript.RunTime.Values;
using VibeScript.RunTime.Values.Interfaces;

namespace VibeScript.RunTime.Environment
{
    public class RuntimeEnvironment
    {
        private RuntimeEnvironment? _parent;
        private Dictionary<string, IRunTimeValue> _variables;
        
        public RuntimeEnvironment(RuntimeEnvironment? parent = null) 
        {
            this._parent = parent;
            this._variables = new Dictionary<string, IRunTimeValue>();
        }

        public IRunTimeValue DeclareVar(string varName, IRunTimeValue value)
        {
            if (_variables.ContainsKey(varName))
            {
                throw new Exception($"Cannot declare variable {varName} as it's already defined.");
            }

            this._variables[varName] = value;
            return value;
        }

        public IRunTimeValue AssignVar(string varName, IRunTimeValue value)
        {
            RuntimeEnvironment env = this.Resolve(varName);
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

    }   
}
