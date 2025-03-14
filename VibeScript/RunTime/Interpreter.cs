using VibeScript.FrontEnd.Ast;
using VibeScript.FrontEnd.Ast.Nodes;
using VibeScript.RunTime.Values;
using VibeScript.RunTime.Values.Interfaces;
using ValueType = VibeScript.RunTime.Values.ValueType;

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
        public IRunTimeValue Evaluate(Statement astNode)
        {
            return astNode.Kind switch
            {
                NodeType.NumericLiteral => new NumberValue(((NumericLiteral)astNode).Value),
                NodeType.NullLiteral => new NullValue(),
                NodeType.BinaryExpr => EvaluateBinaryExpr((BinaryExpr)astNode),
                NodeType.Program => EvaluateProgram((ProgramNode)astNode),
                _ => throw new NotImplementedException($"This AST Node has not yet been setup for interpretation: {astNode}")
            };
        }

        /// <summary>
        /// Evaluates a binary expression (e.g., arithmetic operations like 1 + 2).
        /// </summary>
        private IRunTimeValue EvaluateBinaryExpr(BinaryExpr binop)
        {
            var lhs = Evaluate(binop.Left);
            var rhs = Evaluate(binop.Right);

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
        private IRunTimeValue EvaluateProgram(ProgramNode program)
        {
            IRunTimeValue lastEvaluated = new NullValue();

            foreach (var statement in program.Body)
            {
                lastEvaluated = Evaluate(statement);
            }

            return lastEvaluated;
        }
    }
}