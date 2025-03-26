using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibeScript.FrontEnd.Enums;
using VibeScript.FrontEnd.Interfaces;

namespace VibeScript.FrontEnd
{
    /// <summary>
    /// Responsible for converting source code into tokens.
    /// </summary>
    public static class Lexer
    {
        private static readonly Dictionary<string, TokenType> KeyWords = new()
        {
            { "bet", TokenType.Bet },
            { "lockedIn", TokenType.LockedIn },
            { "cook", TokenType.Cook }
        };

        private static readonly HashSet<char> Skippables = new() { ' ', '\n', '\t', '\r' };
        private static readonly HashSet<char> SingleCharTokens = new() { 
            '(', ')', '{', '}', '[', ']', '=', ';', ':', ',', '.' 
        };
        private static readonly HashSet<char> Operators = new() { '+', '-', '*', '/', '%' };

        /// <summary>
        /// Converts the source code into a list of tokens.
        /// </summary>
        /// <param name="sourceCode">The source code to tokenize.</param>
        /// <returns>A list of tokens representing the source code.</returns>
        public static List<IToken> Tokenize(string sourceCode) 
        {
            List<IToken> tokens = new();
            
            // Create a char array for better performance than Queue
            char[] src = sourceCode.ToCharArray();
            int position = 0;
            int length = src.Length;

            while (position < length)
            {
                char current = src[position];

                // Handle comments
                if (position < length - 1 && current == '/' && src[position + 1] == '/')
                {
                    // Skip to the end of the line
                    while (position < length && src[position] != '\n')
                    {
                        position++;
                    }
                    continue;
                }

                // Process single character tokens
                if (SingleCharTokens.Contains(current))
                {
                    TokenType type = current switch
                    {
                        '(' => TokenType.OpenParen,
                        ')' => TokenType.CloseParen,
                        '{' => TokenType.OpenBrace,
                        '}' => TokenType.CloseBrace,
                        '[' => TokenType.OpenBracket,
                        ']' => TokenType.CloseBracket,
                        '=' => TokenType.Equals,
                        ';' => TokenType.Semicolon,
                        ':' => TokenType.Colon,
                        ',' => TokenType.Comma,
                        '.' => TokenType.Dot,
                        _ => throw new InvalidOperationException($"Unexpected character: {current}")
                    };
                    tokens.Add(CreateToken(current.ToString(), type));
                    position++;
                }
                // Process operators
                else if (Operators.Contains(current))
                {
                    tokens.Add(CreateToken(current.ToString(), TokenType.BinaryOperator));
                    position++;
                }
                else if (char.IsDigit(current))
                {
                    // Build number token
                    int start = position;
                    while (position < length && char.IsDigit(src[position]))
                    {
                        position++;
                    }
                    string number = sourceCode.Substring(start, position - start);
                    tokens.Add(CreateToken(number, TokenType.Number));
                }
                else if (char.IsLetter(current))
                {
                    // Build identifier or keyword token
                    int start = position;
                    while (position < length && (char.IsLetterOrDigit(src[position]) || src[position] == '_'))
                    {
                        position++;
                    }
                    string identifier = sourceCode.Substring(start, position - start);
                    
                    if (KeyWords.TryGetValue(identifier, out TokenType reserved))
                    {
                        tokens.Add(CreateToken(identifier, reserved));
                    }
                    else
                    {
                        tokens.Add(CreateToken(identifier, TokenType.Identifier));
                    }
                }
                else if (Skippables.Contains(current))
                {
                    // Skip whitespace characters
                    position++;
                }
                else
                {
                    throw new InvalidOperationException($"Unrecognized character in source: {current}");
                }
            }

            tokens.Add(CreateToken("EndOfFile", TokenType.EOF));
            return tokens;
        }

        private static Token CreateToken(string value, TokenType type)
        {
            return new Token() { Value = value, Type = type };
        }
    }
}
