using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast.Nodes
{
    public class Property : Expression
    {
        public Property(string key, Expression? value = null) 
        {
            this.Key = key;
            this.Value = value;
        }
        public override NodeType Kind => NodeType.Property;
        public string Key { get; set; }
        public Expression? Value { get; set; }
    }
}
