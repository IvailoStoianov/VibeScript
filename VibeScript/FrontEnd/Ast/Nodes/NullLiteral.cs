using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast.Nodes
{
    public class NullLiteral : Expression
    {
        public NullLiteral() { }
        public override NodeType Kind => NodeType.NullLiteral;
        public object? Value => null;
    }
}
