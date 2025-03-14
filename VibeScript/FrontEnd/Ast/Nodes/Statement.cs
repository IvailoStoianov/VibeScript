using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibeScript.FrontEnd.Ast.Interfaces;

namespace VibeScript.FrontEnd.Ast.Nodes
{
    public abstract class Statement : INode
    {
        public abstract NodeType Kind { get; }

    }
}
