using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibeScript.FrontEnd.Interfaces;
using VibeScript.FrontEnd;
using VibeScript.FrontEnd.Enums;
using VibeScript.FrontEnd.Ast.Nodes;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace VibeScript.FrontEnd.Parser
{
    public class Parser
    {
        private List<IToken> _tokens = new List<IToken>();

        public ProgramNode ProduceAST (string sourceCode)
        {
            this._tokens = Lexer.Tokenize(sourceCode);
            ProgramNode program = new ProgramNode();

            //Parse until end of file
            while(this.NotEOF())
            {
                program.Body.Add(this.ParseStmt());
            }

            return program;
        }
        //Checks if its at the end of a file (EOF)
        private bool NotEOF()
        {
            return this._tokens[0].Type != TokenType.EOF;
        }
        private IToken AtZero()
        {
            return this._tokens[0];
        }

        private Statement ParseStmt()
        {
            //Skip to parse expr
            switch(AtZero().Type)
            {
                case TokenType.Bet:
                case TokenType.LockedIn:
                    return this.ParseVarDeclaration();
                default:
                    return this.ParseExpr();
            }
        }

        private Statement ParseVarDeclaration()
        {
            bool isLockedIn = this.Next().Type == TokenType.LockedIn;
            string identifierName = 
                this.Expect(TokenType.Identifier, "Expected indentifier name following bet | lockedIn keywords.")
                .Value;
            if (this.AtZero().Type == TokenType.Semicolon) 
            {
                this.Next();
                if (isLockedIn)
                {
                    throw new InvalidOperationException("Must assigne value to a lockedIn expression. No value provided.");
                }

                return new VarDeclaration() { IndentifierName = identifierName, IsLockedIn = isLockedIn};
            }

            this.Expect(TokenType.Equals, "Expected equals token following indentifier in var declaration.");
            VarDeclaration declaration = new VarDeclaration() { Value = this.ParseExpr(), IsLockedIn = isLockedIn, IndentifierName = identifierName };

            this.Expect(TokenType.Semicolon, "Statements must end with semicolon.");
            return declaration;
        }

        //More presicidence = further down the tree
        //Orders of prescidence
        //AdditiveExpr
        //MultiplicativeExpr
        //PrimaryExpr
        private Expression ParseExpr()
        {
            return this.ParseAssignmentExpr();
        }

        private Expression ParseAssignmentExpr()
        {
            Expression left = this.ParseAdditiveExpr(); // switch to objects in the future

            if(this.AtZero().Type == TokenType.Equals)
            {
                this.Next(); // Advance past equals
                Expression value = this.ParseAssignmentExpr();
                return new AssignmentExpr(left, value);
            }
            return left; 
        }

        /// <summary>
        /// Parses an additive expression, ensuring left-hand precedence.
        /// This function is responsible for handling expressions involving 
        /// addition (+) and subtraction (-) with left-associative evaluation.
        ///
        /// Example:
        /// Given the input: 10 + 5 - 5
        /// The function will parse it as: (10 + 5) - 5
        /// instead of: 10 + (5 - 5)
        ///
        /// The function starts by parsing a primary expression (number, variable, etc.).
        /// Then, while the next token is '+' or '-', it continuously builds a 
        /// left-associative binary expression tree.
        ///
        /// Returns:
        /// - An `Expression` representing the fully parsed additive expression.
        /// </summary>
        private Expression ParseAdditiveExpr()
        {
            // Parse the first primary expression (left operand)
            Expression left = this.ParseMultiplicativeExpr();

            // Continue parsing while the next token is '+' or '-'
            while (this.AtZero().Value == "+"
                || this.AtZero().Value == "-")
            {
                // Store the operator ('+' or '-')
                string operatorValue = this.Next().Value;

                // Parse the next primary expression (right operand)
                Expression right = this.ParseMultiplicativeExpr();

                // Create a binary expression combining the left and right operands
                BinaryExpr binop = new BinaryExpr(left, operatorValue, right);

                // Update 'left' to maintain left-associativity
                left = binop;
            }

            // Return the fully parsed expression
            return left;
        }

        private Expression ParseMultiplicativeExpr()
        {
            Expression left = this.ParsePrimaryExpr();

            while (this.AtZero().Value == "/"
                || this.AtZero().Value == "*"
                || this.AtZero().Value == "%")
            {
                string operatorValue = this.Next().Value;
                Expression right = this.ParsePrimaryExpr();

                BinaryExpr binop = new BinaryExpr(left, operatorValue, right);
                left = binop;
            }

            return left;
        }
        //TODO: Update the error for closing paren 
        private Expression ParsePrimaryExpr()
        {
            TokenType token = this.AtZero().Type;
            
            switch(token)
            {
                case TokenType.Identifier:
                    return new Identifier(this.Next().Value);
                case TokenType.Number:
                    return new NumericLiteral(float.Parse(this.Next().Value));
                case TokenType.OpenParen:
                    this.Next(); //remove opening paren
                    var value = this.ParseExpr();
                    this.Expect(TokenType.CloseParen, "No closing parenthesis found!"); //remove closing paren
                    return value;
                default:
                    throw new InvalidOperationException($"Unexpected token found during parsing! {JsonConvert.SerializeObject(this.AtZero())}");
            }
        }
        private IToken Next()
        {
            var prev = this._tokens[0];
            _tokens.RemoveAt(0);
            return prev;
        }
        private IToken Expect(TokenType type, string exMessage)
        {
            var prev = this.Next();

            if (prev == null || prev.Type != type) // Check if type does NOT match
            {
                throw new InvalidOperationException(exMessage); 
            }

            return prev; // Return the token if it's valid
        }

    }
}
