using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast.Nodes
{
    public class CallExpr : Expression
    {
        public CallExpr(Expression caller, List<Expression> arguments) 
        {
            this.Caller = caller;
            this.Arguments = arguments;
        }
        public override NodeType Kind => NodeType.CallExpr;
        public List<Expression> Arguments { get; set; } = new();
        public Expression Caller {  get; set; }
    }
}
