﻿using System;
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
        Cook, //this is for function creation

        //Grouping * Operators
        Equals,
        Dot,
        Comma,
        Colon,
        Semicolon,
        OpenParen, // (
        CloseParen, // )
        OpenBrace, // {
        CloseBrace, // }
        OpenBracket, // [
        CloseBracket, // ]
        BinaryOperator,
        EOF, //Singified the end of a file
    }
}
