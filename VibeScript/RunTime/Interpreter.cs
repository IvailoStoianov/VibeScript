using VibeScript.FrontEnd.Ast;
using VibeScript.FrontEnd.Ast.Nodes;
using VibeScript.RunTime.Values;
using VibeScript.RunTime.Values.Interfaces;
using ValueType = VibeScript.RunTime.Values.ValueType;
using VibeScript.RunTime.Environment;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json.Converters;
using System.Text.Json;

namespace VibeScript.RunTime
{
    /// <summary>
    /// Responsible for evaluating an Abstract Syntax Tree (AST) and executing the code.
    /// </summary>
    public class Interpreter
    {
        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter> { new RuntimeValueJsonConverter() }
        };

        /// <summary>
        /// Evaluates a given AST node and returns the corresponding runtime value.
        /// </summary>
        /// <param name="astNode">The AST node to evaluate.</param>
        /// <param name="env">The runtime environment for variable resolution.</param>
        /// <returns>The computed IRunTimeValue based on the AST node type.</returns>
        /// <exception cref="NotImplementedException">Thrown if the node type is not supported.</exception>
        public IRunTimeValue Evaluate(Statement astNode, RuntimeEnvironment env)
        {
            try
            {
                return astNode.Kind switch
                {
                    NodeType.NumericLiteral => new NumberValue(((NumericLiteral)astNode).Value),
                    NodeType.Identifier => EvaluateIdentifier((Identifier)astNode, env),
                    NodeType.ObjectLiteral => EvaluateObjectExpr((ObjectLiteral)astNode, env),
                    NodeType.CallExpr => EvaluateCallExpr((CallExpr)astNode, env),
                    NodeType.BinaryExpr => EvaluateBinaryExpr((BinaryExpr)astNode, env),
                    NodeType.AssignmentExpr => EvaluateAssignmentExpr((AssignmentExpr)astNode, env),
                    NodeType.Program => EvaluateProgram((ProgramNode)astNode, env),
                    NodeType.VarDeclaration => EvaluateVarDeclaration((VarDeclaration)astNode, env),
                    NodeType.FunctionDeclaration => EvaluateFunctionDeclaration((FunctionDeclaration)astNode, env),
                    NodeType.MemberExpr => EvaluateMemberExpr((MemberExpr)astNode, env),
                    _ => throw new NotImplementedException($"This AST Node has not yet been setup for interpretation: {astNode.Kind}")
                };
            }
            catch (Exception ex) when (
                ex is not NotImplementedException && 
                ex is not InvalidOperationException && 
                ex is not DivideByZeroException)
            {
                throw new RuntimeException($"Runtime error evaluating {astNode.Kind}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Evaluates a function declaration statement.
        /// </summary>
        private IRunTimeValue EvaluateFunctionDeclaration(FunctionDeclaration functionDeclaration, RuntimeEnvironment env)
        {
            FuncValue fn = new(
                functionDeclaration.Name,
                functionDeclaration.Parameters,
                env,
                functionDeclaration.Body
            );

            return env.DeclareVar(functionDeclaration.Name, fn, true);
        }

        /// <summary>
        /// Evaluates a variable declaration statement.
        /// </summary>
        private IRunTimeValue EvaluateVarDeclaration(VarDeclaration varDeclaration, RuntimeEnvironment env)
        {
            IRunTimeValue value = varDeclaration.Value != null 
                ? Evaluate(varDeclaration.Value, env) 
                : new NullValue();
                
            return env.DeclareVar(varDeclaration.IndentifierName, value, varDeclaration.IsLockedIn);
        }

        /// <summary>
        /// Evaluates an identifier to retrieve its value from the environment.
        /// </summary>
        private IRunTimeValue EvaluateIdentifier(Identifier ident, RuntimeEnvironment env)
        {
            return env.LookUpVar(ident.Symbol);
        }

        /// <summary>
        /// Evaluates a binary expression (e.g., arithmetic operations like 1 + 2).
        /// </summary>
        private IRunTimeValue EvaluateBinaryExpr(BinaryExpr binop, RuntimeEnvironment env)
        {
            var lhs = Evaluate(binop.Left, env);
            var rhs = Evaluate(binop.Right, env);

            if (lhs.Type != ValueType.Number || rhs.Type != ValueType.Number)
            {
                throw new InvalidOperationException(
                    $"Binary operation '{binop.Operator}' can only be applied to numbers, " +
                    $"got '{lhs.Type}' and '{rhs.Type}'");
            }

            return EvaluateNumericBinaryExpr((NumberValue)lhs, (NumberValue)rhs, binop.Operator);
        }

        /// <summary>
        /// Evaluates a numeric binary expression (e.g., addition, subtraction, multiplication, division).
        /// </summary>
        /// <exception cref="DivideByZeroException">Thrown when dividing or modding by zero.</exception>
        /// <exception cref="NotImplementedException">Thrown for unsupported operators.</exception>
        private NumberValue EvaluateNumericBinaryExpr(NumberValue lhs, NumberValue rhs, string operatorValue)
        {
            double result = operatorValue switch
            {
                "+" => lhs.Value + rhs.Value,
                "-" => lhs.Value - rhs.Value,
                "*" => lhs.Value * rhs.Value,
                "/" => rhs.Value != 0 
                    ? lhs.Value / rhs.Value 
                    : throw new DivideByZeroException("Cannot divide by zero."),
                "%" => rhs.Value != 0 
                    ? lhs.Value % rhs.Value 
                    : throw new DivideByZeroException("Cannot mod by zero."),
                _ => throw new NotImplementedException($"Operator '{operatorValue}' is not supported.")
            };
            
            return new NumberValue((float)result);
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

        /// <summary>
        /// Evaluates an assignment expression.
        /// </summary>
        private IRunTimeValue EvaluateAssignmentExpr(AssignmentExpr node, RuntimeEnvironment env) 
        {
            if (node.Assigne.Kind != NodeType.Identifier)
            {
                throw new InvalidOperationException(
                    $"Invalid left-hand side in assignment expression: expected identifier, got {node.Assigne.Kind}");
            }
            
            string varName = ((Identifier)node.Assigne).Symbol;
            return env.AssignVar(varName, Evaluate(node.Value, env));
        }

        /// <summary>
        /// Evaluates an object literal expression.
        /// </summary>
        private IRunTimeValue EvaluateObjectExpr(ObjectLiteral obj, RuntimeEnvironment env)
        {
            ObjectValue result = new(new Dictionary<string, IRunTimeValue>());
            
            foreach (Property prop in obj.Properties) 
            {
                // Handle shorthand properties like { foo } which is equivalent to { foo: foo }
                IRunTimeValue runtimeVal = prop.Value == null 
                    ? env.LookUpVar(prop.Key) 
                    : Evaluate(prop.Value, env);
                    
                result.Properties.Add(prop.Key, runtimeVal);
            }
            
            return result;
        }

        /// <summary>
        /// Evaluates a function call expression.
        /// </summary>
        private IRunTimeValue EvaluateCallExpr(CallExpr expr, RuntimeEnvironment env)
        {
            // Evaluate the arguments
            RunTimeValue[] args = expr.Arguments
                .Select(arg => (RunTimeValue)Evaluate(arg, env))
                .ToArray();

            // Evaluate the caller (function) of the expression
            IRunTimeValue fn = Evaluate(expr.Caller, env);

            // Handle native functions (built-ins)
            if (fn is NativeFuncValue nativeFn)
            {
                return nativeFn.Call(args, env);
            }
            
            // Handle user-defined functions
            if (fn is FuncValue func)
            {
                // Create a new scope for the function execution
                RuntimeEnvironment scope = new(func.Environment);

                // Bind parameters to arguments
                for (int i = 0; i < func.Parameters.Count; i++)
                {
                    // TODO: Add arity checking (function parameter count vs argument count)
                    if (i < args.Length)
                    {
                        string varName = func.Parameters[i];
                        scope.DeclareVar(varName, args[i], false);
                    }
                }
                
                // Evaluate the function body line by line
                IRunTimeValue result = new NullValue();
                foreach (Statement stmt in func.Body) 
                {
                    result = Evaluate(stmt, scope);
                }
                
                // Return the result of evaluating the last statement
                return result;
            }
            
            throw new InvalidOperationException(
                $"Cannot call a non-function value of type '{fn.Type}'");
        }

        /// <summary>
        /// Evaluates a member expression (object property access).
        /// </summary>
        private IRunTimeValue EvaluateMemberExpr(MemberExpr expr, RuntimeEnvironment env)
        {
            IRunTimeValue obj = Evaluate(expr.Object, env);
            
            if (obj is not ObjectValue objValue)
            {
                throw new InvalidOperationException(
                    $"Cannot access properties of a non-object value of type '{obj.Type}'");
            }

            // Handle computed properties like obj[expr] vs dot notation obj.prop
            string propertyName;
            if (expr.IsComputed)
            {
                // For computed properties, evaluate the property expression
                IRunTimeValue computed = Evaluate(expr.Property, env);
                if (computed is not RunTimeValue val || string.IsNullOrEmpty(val.ToString()))
                {
                    throw new InvalidOperationException($"Invalid property key: {computed}");
                }
                propertyName = val.ToString();
            }
            else
            {
                // For dot notation, property should be an identifier
                if (expr.Property is not Identifier ident)
                {
                    throw new InvalidOperationException(
                        "Non-computed member expression property must be an identifier");
                }
                propertyName = ident.Symbol;
            }

            // Try to get the property value
            if (!objValue.Properties.TryGetValue(propertyName, out IRunTimeValue value))
            {
                return new NullValue(); // Property doesn't exist
            }

            return value;
        }
    }

    /// <summary>
    /// Exception thrown for runtime errors during interpretation.
    /// </summary>
    public class RuntimeException : Exception
    {
        public RuntimeException(string message) : base(message) { }
        public RuntimeException(string message, Exception innerException) : base(message, innerException) { }
    }
}