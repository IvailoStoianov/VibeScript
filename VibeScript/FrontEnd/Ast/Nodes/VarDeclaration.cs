using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast.Nodes
{
    public class VarDeclaration : Statement
    {
        public override NodeType Kind => NodeType.VarDeclaration;
        public bool IsLockedIn { get; set; }
        public required string IndentifierName { get; set; }
        public Expression? Value { get; set; }

    }
}
