using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast.Nodes
{
    public class FunctionDeclaration : Statement
    {
        public FunctionDeclaration(List<string>? parameters, string name, List<Statement> body) 
        {
            this.Parameters = parameters ?? new List<string>();
            this.Name = name;
            this.Body = body;
        }
        public override NodeType Kind => NodeType.FunctionDeclaration;

        public List<string> Parameters { get; set; } = new();
        public string Name { get; set; }
        public List<Statement> Body { get; set; }
    }
}
