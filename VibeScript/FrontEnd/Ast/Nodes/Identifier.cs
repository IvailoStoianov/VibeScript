using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VibeScript.FrontEnd.Ast.Nodes
{
    public class Identifier : Expression
    {
        public Identifier(string symbol)
        {
            Symbol = symbol;
        }
        public override NodeType Kind => NodeType.Identifier;
        public string Symbol { get; set; }

    }
}
