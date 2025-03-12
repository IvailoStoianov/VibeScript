using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibeScript.FrontEnd.Enums;

namespace VibeScript.FrontEnd.Interfaces
{
    public interface IToken
    {
        string? Value { get; set; }
        TokenType Type { get; set; }
    }
}
