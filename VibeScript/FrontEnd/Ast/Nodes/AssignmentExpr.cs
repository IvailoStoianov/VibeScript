using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast.Nodes
{
    public class AssignmentExpr : Expression
    {
        public AssignmentExpr(Expression assigne, Expression value)
        {
            this.Assigne = assigne;
            this.Value = value;
        }
        public override NodeType Kind => NodeType.AssignmentExpr;

        public Expression Assigne { get; set; }

        public Expression Value { get; set; }
    }
}
