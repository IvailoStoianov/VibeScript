using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibeScript.FrontEnd.Enums;
using VibeScript.FrontEnd.Interfaces;

namespace VibeScript.FrontEnd
{
    public static class Lexer
    {
        public static List<IToken> Tokenize(string sourceCode) 
        {
            //Mapping for reserved key words
            Dictionary<string , TokenType> keyWords = new Dictionary<string , TokenType>()
            {
                { "bet", TokenType.Bet },
            };

            List<IToken> tokens = new List<IToken>();
            Queue<char> src = new Queue<char>(sourceCode);

            //TODO: Improve so it doesnt leak memory
            //Build each token until end of file
            while (src.Count > 0)
            {
                if (src.Peek() == '(')
                {
                    tokens.Add(createToken(src.Dequeue().ToString(), TokenType.OpenParen));
                } 
                else if (src.Peek() == ')')
                {
                    tokens.Add(createToken(src.Dequeue().ToString(), TokenType.CloseParen));
                }
                else if (src.Peek() == '+' ||
                    src.Peek() == '-' ||
                    src.Peek() == '*' ||
                    src.Peek() == '/' ||
                    src.Peek() == '%')
                {
                    tokens.Add(createToken(src.Dequeue().ToString(), TokenType.BinaryOperator));
                }
                else if (src.Peek() == '=')
                {
                    tokens.Add(createToken(src.Dequeue().ToString(), TokenType.Equals));
                }
                else
                {
                    //Handle multicharacter tokens

                    //Build number token
                    if (char.IsDigit(src.Peek()))
                    {
                        StringBuilder numb = new StringBuilder();
                        while (src.Count > 0 && char.IsDigit(src.Peek()))
                        {
                            numb.Append(src.Dequeue().ToString());
                        }

                        tokens.Add(createToken(numb.ToString(), TokenType.Number));
                    }
                    //Build identifier tokens
                    else if (char.IsLetter(src.Peek()))
                    {
                        StringBuilder ident = new StringBuilder();
                        while (src.Count > 0 && char.IsLetter(src.Peek()))
                        {
                            ident.Append(src.Dequeue().ToString());
                        }
                        TokenType reserved;
                        // Check if the identifier exists in the keyword dictionary
                        if (keyWords.TryGetValue(ident.ToString(), out reserved))
                        {
                            // If found, add it as a keyword token
                            tokens.Add(createToken(ident.ToString(), reserved));
                        }
                        else
                        {
                            // If not found, treat it as an identifier
                            tokens.Add(createToken(ident.ToString(), TokenType.Identifier));
                        }


                    }
                    else if (isSkippable(src.Peek().ToString()))
                    {
                        src.Dequeue(); // Skip the character
                    }
                    else
                    {
                        Console.WriteLine($"Unrecgonized character found in source: {src.Peek()}");
                        Environment.Exit(1);
                    }
                }
            }

            tokens.Add(createToken("EndOfFile", TokenType.EOF));
            return tokens;
        }
        private static Token createToken(string value, TokenType type)
        {
            return new Token() { Value = value, Type = type };
        }

        //TODO: Could be improved
        private static bool isSkippable(string str)
        {
            return str == " " || str == "\n" || str == "\t";
        } 
    }
}
