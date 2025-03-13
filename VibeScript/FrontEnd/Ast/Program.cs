using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast
{
    public class Program : Statement
    {
        public override string Kind => NodeType.Program;
        public List<Statement> Body { get; set; } = new();
    }
}
