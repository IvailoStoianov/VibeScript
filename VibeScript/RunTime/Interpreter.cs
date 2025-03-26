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
        /// <returns>The computed IRunTimeValue based on the AST node type.</returns>
        /// <exception cref="NotImplementedException">Thrown if the node type is not supported.</exception>
        public IRunTimeValue Evaluate(Statement astNode, RuntimeEnvironment env)
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
                _ => throw new NotImplementedException($"This AST Node has not yet been setup for interpretation: {JsonConvert.SerializeObject(astNode)}")
            };
        }

        private IRunTimeValue EvaluateFunctionDeclaration(FunctionDeclaration functionDeclaration, RuntimeEnvironment env)
        {
            FuncValue fn = new FuncValue(functionDeclaration.Name,functionDeclaration.Parameters,env,functionDeclaration.Body);

            return env.DeclareVar(functionDeclaration.Name, fn, true);
        }

        private IRunTimeValue EvaluateVarDeclaration(VarDeclaration varDeclaration, RuntimeEnvironment env)
        {
            IRunTimeValue value = varDeclaration.Value != null ? Evaluate(varDeclaration.Value, env) : new NullValue();
            return env.DeclareVar(varDeclaration.IndentifierName, value, varDeclaration.IsLockedIn);
        }

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

        private IRunTimeValue EvaluateAssignmentExpr(AssignmentExpr node, RuntimeEnvironment env) 
        {
            if(node.Assigne.Kind != NodeType.Identifier)
            {
                throw new InvalidOperationException($"Invalid LHS inside assigment expression {JsonConvert.SerializeObject(node, Formatting.Indented)}");
            }
            string varName = ((Identifier)node.Assigne).Symbol;
            return env.AssignVar(varName, Evaluate(node.Value, env));
        }

        private IRunTimeValue EvaluateObjectExpr(ObjectLiteral obj, RuntimeEnvironment env)
        {
            ObjectValue crrObject = new ObjectValue( new Dictionary<string, IRunTimeValue>());
            foreach (Property prop in obj.Properties) 
            {
                // { foo: foo }
                IRunTimeValue runtimeVal = prop.Value == null ? env.LookUpVar(prop.Key) : Evaluate(prop.Value, env);
                crrObject.Properties.Add(prop.Key, runtimeVal);
            }
            return crrObject;
        }
        private IRunTimeValue EvaluateCallExpr(CallExpr expr, RuntimeEnvironment env)
        {
            // Evaluate the arguments
            RunTimeValue[] args = expr.Arguments
                                    .Select(arg => (RunTimeValue)Evaluate(arg, env))
                                    .ToArray();

            // Evaluate the caller (function) of the expression
            IRunTimeValue fn = Evaluate(expr.Caller, env);

            // Check if the function is of type 'NativeFunc'
            if (fn is NativeFuncValue nativeFn)
            {
                // Call the native function with the evaluated arguments
                return nativeFn.Call(args, env);
            }
            else if(fn is FuncValue func)
            {
                RuntimeEnvironment scope = new RuntimeEnvironment(func.Environment);

                for (int i = 0; i < func.Parameters.Count; i++)
                {
                    //TODO: Check the bounds here.
                    //Verify arity of function
                    string varName = func.Parameters[i];
                    scope.DeclareVar(varName, args[i], false);
                }
                IRunTimeValue result = new NullValue();
                //Evaluate the function body line by line
                foreach (Statement stmt in func.Body) 
                {
                    result = Evaluate(stmt, scope);
                }
                //this makes it so the function will return the last evaluated statement
                return result;
            }
            else
            {
                // If it's not a function, throw an exception
                throw new InvalidOperationException($"Cannot call a value that's not a function: {JsonConvert.SerializeObject(fn, Formatting.Indented)}.");
            }
        }

        private IRunTimeValue EvaluateMemberExpr(MemberExpr expr, RuntimeEnvironment env)
        {
            IRunTimeValue obj = Evaluate(expr.Object, env);
            
            if (obj is not ObjectValue objValue)
            {
                throw new InvalidOperationException($"Cannot access properties of non-object value: {JsonConvert.SerializeObject(obj, Formatting.Indented)}");
            }

            // Handle computed properties like obj[expr] vs dot notation obj.prop
            string propertyName;
            if (expr.IsComputed)
            {
                // For computed properties, evaluate the property expression
                IRunTimeValue computed = Evaluate(expr.Property, env);
                if (computed is not RunTimeValue val || string.IsNullOrEmpty(val.ToString()))
                {
                    throw new InvalidOperationException($"Invalid property key: {JsonConvert.SerializeObject(computed, Formatting.Indented)}");
                }
                propertyName = val.ToString();
            }
            else
            {
                // For dot notation, property should be an identifier
                if (expr.Property is not Identifier ident)
                {
                    throw new InvalidOperationException("Non-computed member expression property must be an identifier");
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
}