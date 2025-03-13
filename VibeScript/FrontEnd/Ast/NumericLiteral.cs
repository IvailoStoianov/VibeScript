using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast
{
    public class NumericLiteral : Expression
    {
        public NumericLiteral(double value)
        {
            Value = value;
        }
        public override string Kind => NodeType.NumericLiteral;
        public double Value { get; set; }
    }
}
