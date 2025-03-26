using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VibeScript.FrontEnd.Ast
{
    public enum NodeType
    {
        //Statements
        Program,
        VarDeclaration,
        FunctionDeclaration,

        //Expressions
        AssignmentExpr,
        MemberExpr,
        CallExpr,

        //Literals
        ObjectLiteral,
        Property,
        NumericLiteral,
        Identifier,
        BinaryExpr
    }
}
