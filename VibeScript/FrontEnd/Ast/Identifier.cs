using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast
{
    public class Identifier : Expression
    {
        public Identifier(string symbol)
        {
            Symbol = symbol;
        }
        public override string Kind => NodeType.Identifier;
        public string Symbol {  get; set; }
    }
}
