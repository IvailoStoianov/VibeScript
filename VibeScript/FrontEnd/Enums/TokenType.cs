using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Enums
{
    public enum TokenType
    {
        //Literal types
        Number,
        Identifier,

        //Keywords
        Bet,
        LockedIn,

        //Grouping * Operators
        Equals,
        Semicolon,
        OpenParen,
        CloseParen,
        BinaryOperator,
        EOF, //Singified the end of a file
    }
}
