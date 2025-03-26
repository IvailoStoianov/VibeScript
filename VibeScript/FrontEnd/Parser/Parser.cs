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
using VibeScript.FrontEnd.Ast;

namespace VibeScript.FrontEnd.Parser
{
    public class Parser
    {
        private List<IToken> _tokens = new List<IToken>();

        public ProgramNode ProduceAST(string sourceCode)
        {
            this._tokens = Lexer.Tokenize(sourceCode);
            ProgramNode program = new ProgramNode();

            //Parse until end of file
            while (this.NotEOF())
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
            switch (AtZero().Type)
            {
                case TokenType.Bet:
                case TokenType.LockedIn:
                    return this.ParseVarDeclaration();
                case TokenType.Cook:
                    return this.ParseFnDeclaration();
                default:
                    return this.ParseExpr();
            }
        }

        private Statement ParseFnDeclaration()
        {
            this.Next();
            string name = this.Expect(TokenType.Identifier, "Expected function name following cook keyword.").Value;
            List<Expression> args = this.ParseArgs();

            if (args.Any(arg => arg.Kind != NodeType.Identifier))
            {
                throw new InvalidOperationException("All function parameters must be identifiers.");
            }

            List<string> parameters = args
                .Select(arg => ((Identifier)arg).Symbol)
                .ToList();
            this.Expect(TokenType.OpenBrace, "Expected function body following declaration.");

            List<Statement> body = new();
            while (this.AtZero().Type != TokenType.EOF
                && this.AtZero().Type != TokenType.CloseBrace)
            {
                body.Add(this.ParseStmt());
            }
            this.Expect(TokenType.CloseBrace, "Closing brace expected inside function declaration.");

            return new FunctionDeclaration(parameters,name,body);
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

                return new VarDeclaration() { IndentifierName = identifierName, IsLockedIn = isLockedIn };
            }

            this.Expect(TokenType.Equals, "Expected equals token following indentifier in var declaration.");
            VarDeclaration declaration = new VarDeclaration() { Value = this.ParseExpr(), IsLockedIn = isLockedIn, IndentifierName = identifierName };

            this.Expect(TokenType.Semicolon, "Statements must end with semicolon.");
            return declaration;
        }

        //More presicidence = further down the tree
        //Orders of prescidence
        //Assigment
        //Object
        //AdditiveExpr
        //MultiplicativeExpr
        //Call
        //Member
        //PrimaryExpr
        private Expression ParseExpr()
        {
            return this.ParseAssignmentExpr();
        }

        private Expression ParseAssignmentExpr()
        {
            Expression left = this.ParseObjectExpr();

            if (this.AtZero().Type == TokenType.Equals)
            {
                this.Next(); // Advance past equals
                Expression value = this.ParseAssignmentExpr();
                return new AssignmentExpr(left, value);
            }
            return left;
        }

        private Expression ParseObjectExpr()
        {
            if (this.AtZero().Type != TokenType.OpenBrace)
            {
                return ParseAdditiveExpr();
            }

            this.Next();

            List<Property> properties = new List<Property>();

            while (this.NotEOF() && this.AtZero().Type != TokenType.CloseBrace)
            {
                string key = this.Expect(TokenType.Identifier, "Object literal key expected.").Value;

                //Allows shorthand key: pair -> { key, }
                if (this.AtZero().Type == TokenType.Comma)
                {
                    this.Next(); //advance past coma 
                    properties.Add(new Property(key));
                    continue;
                }
                //Allows shorthand key: pair -> { key }
                else if (this.AtZero().Type == TokenType.CloseBrace)
                {
                    properties.Add(new Property(key));
                    continue;
                }

                //Support for { key: val }
                this.Expect(TokenType.Colon, "Missing colon following indentifier in object expression.");

                Expression value = this.ParseExpr();

                properties.Add(new Property(key, value));

                if (this.AtZero().Type != TokenType.CloseBrace)
                {
                    this.Expect(TokenType.Comma, "Expected comma or closing bracket following property.");
                }
            }

            this.Expect(TokenType.CloseBrace, "Object literal missing closing brace.");

            return new ObjectLiteral(properties);
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
            Expression left = this.ParseCallMemberExpr();

            while (this.AtZero().Value == "/"
                || this.AtZero().Value == "*"
                || this.AtZero().Value == "%")
            {
                string operatorValue = this.Next().Value;
                Expression right = this.ParseCallMemberExpr();

                BinaryExpr binop = new BinaryExpr(left, operatorValue, right);
                left = binop;
            }

            return left;
        }
        //foo.x()()
        private Expression ParseCallMemberExpr()
        {
            Expression member = this.ParseMemberExpr();

            if (this.AtZero().Type == TokenType.OpenParen)
            {
                return this.ParseCallExpr(member);
            }

            return member;
        }
        //Allows channing of function calls 
        private Expression ParseCallExpr(Expression caller)
        {
            Expression callExpr = new CallExpr(caller, this.ParseArgs());

            if (this.AtZero().Type == TokenType.OpenParen)
            {
                callExpr = this.ParseCallExpr(callExpr);
            }
            return callExpr;
        }
        private List<Expression> ParseArgs()
        {
            this.Expect(TokenType.OpenParen, "Expected open parenthesis.");
            List<Expression> args =
                this.AtZero().Type == TokenType.CloseParen
                ? new()
                : this.ParseArgsList();
            this.Expect(TokenType.CloseParen, "Missing closing parenthesis inside arguments list.");
            return args;
        }
        //foo(x = 5, y = 2);
        private List<Expression> ParseArgsList()
        {
            List<Expression> args = new List<Expression>();
            args.Add(ParseAssignmentExpr());
            while (this.AtZero().Type == TokenType.Comma && this.Next() != null)
            {
                args.Add(ParseAssignmentExpr());
            }
            return args;
        }
        private Expression ParseMemberExpr()
        {
            Expression obj = this.ParsePrimaryExpr();
            while (this.AtZero().Type == TokenType.Dot || this.AtZero().Type == TokenType.OpenBracket)
            {
                IToken crrOperator = this.Next();
                Expression property;
                bool isComputed;

                //non-computed values (obj.expr)
                if (crrOperator.Type == TokenType.Dot)
                {
                    //get identifier
                    property = this.ParsePrimaryExpr();
                    isComputed = false;

                    if (property.Kind != NodeType.Identifier)
                    {
                        throw new InvalidOperationException("Cannot use dot operator without the inside being an identifier.");
                    }
                }
                else //this allows obj[computedValue]
                {
                    isComputed = true;
                    property = this.ParsePrimaryExpr();
                    this.Expect(TokenType.CloseBracket, "Missing closing bracket in computed value.");
                }
                obj = new MemberExpr(obj, property, isComputed);
            }
            return obj;
           
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
