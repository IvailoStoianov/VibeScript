using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibeScript.FrontEnd.Enums;
using VibeScript.FrontEnd.Interfaces;

namespace VibeScript.FrontEnd
{
    public class Lexer
    {
        public List<IToken> Tokenize(string sourceCode) 
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
                    src.Peek() == '/')
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
                        //Indentifier because its not in keywords
                        if (!keyWords.TryGetValue(ident.ToString(), out reserved))
                        {
                            tokens.Add(createToken(ident.ToString(), TokenType.Identifier));
                        }
                        else
                        {
                            tokens.Add(createToken(ident.ToString(), reserved));
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

            return tokens;
        }
        private Token createToken(string value, TokenType type)
        {
            return new Token() { Value = value, Type = type };
        }

        //TODO: Could be improved
        private bool isSkippable(string str)
        {
            return str == " " || str == "\n" || str == "\t";
        } 
    }
}
