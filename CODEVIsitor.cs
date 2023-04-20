using System.Diagnostics;
using CODEInterpreter_v1.Utils;
using static CODEInterpreter_v1.Utils.CODEParser;

namespace CODEInterpreter_v1;

public class CODEVisitor : CODEBaseVisitor<object?>
{
    private Dictionary<string, object> Variables { get; } = new();

    public override object? VisitProgram(ProgramContext context)
    {
        foreach (var line in context.line())
        {
            Visit(line);
        }

        return null;
    }

    public override object? VisitLine(LineContext context)
    {
        return Visit(context.GetChild(0));
    }
    


    public override object VisitAssignment(CODEParser.AssignmentContext context)
    {
        var identifier = context.IDENTIFIER().GetText();
        var expression = context.expression();

        if (!Variables.ContainsKey(identifier))
            throw new Exception($"Variable '{identifier}' not found");

        foreach (var expr in expression)
        {
            object value = Visit(expr)!;
            Variables[identifier] = value;
            Console.Write(Variables[identifier]);
        }

        return null;
    }

    public override object VisitPrint(CODEParser.PrintContext context)
    {
        var expression = Visit(context.expression());
        Debug.WriteLine(expression);

        return null;
    }

    public override object VisitDeclaration(CODEParser.DeclarationContext context)
    {
        var identifier = context.IDENTIFIER().ToString();
        if (Variables.ContainsKey(identifier))
            throw new Exception($"Variable '{identifier}' already exists");

        Variables.Add(identifier, null);

        return null;
    }

    //private object Visit(ExpressionContext[] expressionContexts)
    //{
    //    var expression = Visit(expressionContexts[0]);
    //    for (var i = 1; i < expressionContexts.Length; i++)
    //    {
    //        var nextExpression = Visit(expressionContexts[i]);
    //        expression = expression switch
    //        {
    //            int left when nextExpression is int right => left + right,
    //            string left when nextExpression is string right => left + right,
    //            _ => throw new Exception("Invalid expression")
    //        };
    //    }

    //    return expression;
    //}

    //public override object VisitExpression(CODEParser.ExpressionContext context)
    //{
    //    if (context.GetChildCount() == 1)
    //    {
    //        return Visit(context.GetChild(0));
    //    }

    //    var left = Visit(context.expression(0));
    //    var right = Visit(context.expression(1));

    //    return context.GetChild(1).GetText() switch
    //    {
    //        "+" => (int)left + (int)right,
    //        "-" => (int)left - (int)right,
    //        "*" => (int)left * (int)right,
    //        "/" => (int)left / (int)right,
    //        _ => throw new Exception("Unknown operator")
    //    };
    //}
}