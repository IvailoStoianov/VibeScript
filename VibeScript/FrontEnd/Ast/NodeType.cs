using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast
{
    public static class NodeType
    {
        public const string Program = "Program";
        public const string NumericLiteral = "NumericLiteral";
        public const string Identifier = "Identifier";
        public const string BinaryExpr = "BinaryExpr";
    }
}
