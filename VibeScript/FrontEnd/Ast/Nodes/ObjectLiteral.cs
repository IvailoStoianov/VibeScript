using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast.Nodes
{
    public class ObjectLiteral : Expression
    {
        public ObjectLiteral(List<Property> properties) 
        {
            this.Properties = properties;
        }
        public override NodeType Kind => NodeType.ObjectLiteral;
        public List<Property> Properties { get; set; } = new();
    }
}
