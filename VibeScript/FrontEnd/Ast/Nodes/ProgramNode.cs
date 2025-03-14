using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast.Nodes
{
    public class ProgramNode : Statement
    {
        public override NodeType Kind => NodeType.Program;
        public List<Statement> Body { get; set; } = new();
    }
}
