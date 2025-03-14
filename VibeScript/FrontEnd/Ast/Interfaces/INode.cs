using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast.Interfaces
{
    public interface INode
    {
        NodeType Kind { get; }
    }
}
