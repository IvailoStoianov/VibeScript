using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast.Nodes
{
    public class NumericLiteral : Expression
    {
        public NumericLiteral(float value)
        {
            Value = value;
        }
        public override NodeType Kind => NodeType.NumericLiteral;
        public float Value { get; set; }
    }
}
