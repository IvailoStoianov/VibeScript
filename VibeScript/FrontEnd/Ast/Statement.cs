using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast
{
    public abstract class Statement
    {
        public abstract string Kind { get; }
    }
}
