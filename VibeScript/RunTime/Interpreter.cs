using VibeScript.FrontEnd.Ast;
using VibeScript.FrontEnd.Ast.Nodes;
using VibeScript.RunTime.Values;
using VibeScript.RunTime.Values.Interfaces;
using ValueType = VibeScript.RunTime.Values.ValueType;
using VibeScript.RunTime.Environment;

namespace VibeScript.RunTime
{
    /// <summary>
    /// Responsible for evaluating an Abstract Syntax Tree (AST) and executing the code.
    /// </summary>
    public class Interpreter
    {
        /// <summary>
        /// Evaluates a given AST node and returns the corresponding runtime value.
        /// </summary>
        /// <param name="astNode">The AST node to evaluate.</param>
        /// <returns>The computed IRunTimeValue based on the AST node type.</returns>
        /// <exception cref="NotImplementedException">Thrown if the node type is not supported.</exception>
        public IRunTimeValue Evaluate(Statement astNode, RuntimeEnvironment env)
        {
            return astNode.Kind switch
            {
                NodeType.NumericLiteral => new NumberValue(((NumericLiteral)astNode).Value),
                NodeType.Identifier => EvaluateIdentifier((Identifier)astNode, env),
                NodeType.NullLiteral => new NullValue(),
                NodeType.BinaryExpr => EvaluateBinaryExpr((BinaryExpr)astNode, env),
                NodeType.Program => EvaluateProgram((ProgramNode)astNode, env),
                _ => throw new NotImplementedException($"This AST Node has not yet been setup for interpretation: {astNode}")
            };
        }

        private IRunTimeValue EvaluateIdentifier(Identifier ident, RuntimeEnvironment env)
        {
            var val = env.LookUpVar(ident.Symbol);
            return val;
        }

        /// <summary>
        /// Evaluates a binary expression (e.g., arithmetic operations like 1 + 2).
        /// </summary>
        private IRunTimeValue EvaluateBinaryExpr(BinaryExpr binop, RuntimeEnvironment env)
        {
            var lhs = Evaluate(binop.Left, env);
            var rhs = Evaluate(binop.Right, env);

            return (lhs.Type, rhs.Type) switch
            {
                (ValueType.Number, ValueType.Number) => EvaluateNumericBinaryExpr((NumberValue)lhs, (NumberValue)rhs, binop.Operator),
                _ => new NullValue() // Error handling for unsupported types.
            };
        }

        /// <summary>
        /// Evaluates a numeric binary expression (e.g., addition, subtraction, multiplication, division).
        /// </summary>
        /// <exception cref="DivideByZeroException">Thrown when dividing or modding by zero.</exception>
        /// <exception cref="NotImplementedException">Thrown for unsupported operators.</exception>
        private NumberValue EvaluateNumericBinaryExpr(NumberValue lhs, NumberValue rhs, string operatorValue)
        {
            return new NumberValue(operatorValue switch
            {
                "+" => lhs.Value + rhs.Value,
                "-" => lhs.Value - rhs.Value,
                "*" => lhs.Value * rhs.Value,
                "/" => rhs.Value != 0 ? lhs.Value / rhs.Value : throw new DivideByZeroException("Cannot divide by zero."),
                "%" => rhs.Value != 0 ? lhs.Value % rhs.Value : throw new DivideByZeroException("Cannot mod by zero."),
                _ => throw new NotImplementedException($"Operator '{operatorValue}' is not supported.")
            });
        }

        /// <summary>
        /// Evaluates a program node by sequentially evaluating all statements in its body.
        /// </summary>
        private IRunTimeValue EvaluateProgram(ProgramNode program, RuntimeEnvironment env)
        {
            IRunTimeValue lastEvaluated = new NullValue();

            foreach (var statement in program.Body)
            {
                lastEvaluated = Evaluate(statement, env);
            }

            return lastEvaluated;
        }
    }
}