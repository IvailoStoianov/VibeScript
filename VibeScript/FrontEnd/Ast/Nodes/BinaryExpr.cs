using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast.Nodes
{
    public class BinaryExpr : Expression
    {
        public BinaryExpr(Expression left, string op, Expression right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }
        public override NodeType Kind => NodeType.BinaryExpr;
        public Expression Left { get; set; }
        public Expression Right { get; set; }
        public string Operator { get; set; }
    }
}
