using VibeScript.RunTime.Values;
using VibeScript.RunTime.Values.Interfaces;

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

        
    }   
}
