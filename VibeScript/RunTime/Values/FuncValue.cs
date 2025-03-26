using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibeScript.FrontEnd.Ast.Nodes;
using VibeScript.RunTime.Environment;
using static VibeScript.RunTime.Values.RunTimeValue;

namespace VibeScript.RunTime.Values
{
    public class FuncValue : RunTimeValue
    {
        public FuncValue(string name, List<string>? parameters, RuntimeEnvironment environment, List<Statement> body)
        {
            this.Name = name;
            this.Parameters = parameters ?? new List<string>();
            this.Environment = environment;
            this.Body = body;
        }

        public override ValueType Type => ValueType.Func;

        public string Name {  get; set; }
        public List<string> Parameters { get; set; } = new();
        public RuntimeEnvironment Environment { get; set; }
        public List<Statement> Body { get; set; } = new();   
    }
}
