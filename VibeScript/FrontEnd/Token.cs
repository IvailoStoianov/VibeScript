using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibeScript.FrontEnd.Enums;
using VibeScript.FrontEnd.Interfaces;

namespace VibeScript.FrontEnd
{
    public class Token : IToken
    {
        public string? Value { get; set; }
        public TokenType Type { get; set; }
    }
}
