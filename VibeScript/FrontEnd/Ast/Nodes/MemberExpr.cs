using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast.Nodes
{
    public class MemberExpr : Expression
    {
        public MemberExpr(Expression obj, Expression prop, bool isComputed) 
        { 
            this.Object = obj;
            this.Property = prop;
            this.IsComputed = isComputed;
        }
        public override NodeType Kind => NodeType.MemberExpr;
        public Expression Object { get; set; }
        public Expression Property { get; set; }
        public bool IsComputed { get; set; }
    }
}
