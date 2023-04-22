using CODEInterpreter_v1.Utils;
using System.Text; 
using System.Text.RegularExpressions;
namespace CODEInterpreter_v1;

public class SpeakVisitor : SpeakBaseVisitor<object?>
{
    
   private Dictionary<string, string[]> Globalvariables = new ();
   private readonly List<string> _reservedKeywords = new List<string>()
   {
       "SCAN",
       "DISPLAY",
       "INT",
       "CHAR",
       "FLOAT",
       "BOOL",
       "AND",
       "OR",
       "NOT",
       "IF",
       "ELSE",
       "WHILE"
   };
   
    public Boolean CheckVariables(string variableName)
    {
        if (Globalvariables.ContainsKey(variableName))
        {
            return true;
        }
        else
        {
            return false;
        }

        
    }

    public Boolean CheckValue(string dataType, string value)
    {
        string wholeNumber = @"^(?:[+-]?\d+|\d+)$";
        string decimalNumber = @"(^[+-]?\d*\.\d+$)|(^(?:[+-]?\d+|\d+)$)";
        string charValue = @"^([a-zA-Z])$";
        string boolValue = @"True|False";
           
        if (dataType == "INT")
        {
           return Regex.IsMatch(value.Trim(), wholeNumber);
            
        }
        else if (dataType == "FLOAT")
        {
            return Regex.IsMatch(value.Trim(), decimalNumber);
        }
        else if (dataType == "CHAR")
        {
            return Regex.IsMatch(value.Trim(), charValue);
        }
        else if (dataType == "BOOL")
        {
            return Regex.IsMatch(value.Trim(), boolValue);
        }
        else
        {
            throw new Exception("Invalid Datatype");
        }


    }

    public void StoreVariable(string variableName, string dataType, string expression, string value)
    {
        bool valid = false;
        bool notArithmetic = false;
        
        notArithmetic =  Regex.IsMatch( expression, @"^[+\-]?(\d+|\d*\.\d+)$");
        //check if arithmetic
        //if it is get value and get whole number
        if ((dataType == "INT" || dataType == "FLOAT" || dataType == "CHAR" || dataType == "BOOL"))
        {
            //if it is arithmetic and int truncate it
            if (dataType == "INT" && !notArithmetic)
            {
                double number = double.Parse(value);
                int intNumber = (int)Math.Truncate(number);
                value = intNumber.ToString();
            }
            if ((dataType == "INT" || dataType == "FLOAT") && notArithmetic)
            {
              
                value = expression.ToString();
            }
            
            
            valid = CheckValue(dataType, value);
            if (valid)
            {
                Globalvariables.Add(variableName, new[] { dataType, value });
            }
            else
            {
               
                throw new Exception("Invalid Value");
            }
        }
        else
        {
            throw new Exception("Invalid DataType");
        }
    }
    public void AssignVariable(string variableName, string dataType, string expression, string value)
    {
        bool valid = false;
        bool notArithmetic = false;
        
        notArithmetic =  Regex.IsMatch( expression, @"^[+\-]?(\d+|\d*\.\d+)$");
        //check if arithmetic
        //if it is get value and get whole number
        if ((dataType == "INT" || dataType == "FLOAT" || dataType == "CHAR" || dataType == "BOOL"))
        {
            //if it is arithmetic and int truncate it
            if (dataType == "INT" && !notArithmetic)
            {
                double number = double.Parse(value);
                int intNumber = (int)Math.Truncate(number);
                value = intNumber.ToString();
            }
            if ((dataType == "INT" || dataType == "FLOAT") && notArithmetic)
            {
              
                value = expression.ToString();
            }
            valid = CheckValue(dataType, value);
          
            if (valid)
            {
                Globalvariables[variableName] = new[] { dataType, value };
                    
            }
            else
            {
               
                throw new Exception("Invalid Value");
            }
        }
        else
        {
            throw new Exception("Invalid DataType");
        }
    }
    public override object? VisitDeclaration(SpeakParser.DeclarationContext context)
    {
        string dataType = context.dataType().GetText();
        
        foreach (SpeakParser.VariableContext varCtx in context.variable())
        {
           
            string variableName = varCtx.IDENTIFIER().GetText();
            
            //check if variable is a keyword
            if (_reservedKeywords.Contains(variableName))
                throw new Exception("Variable name is a keyword");
            
           
            var variableValue = varCtx.expression() != null ? Visit(varCtx.expression()) : "";

            
            //if variable value is empty or just initialization
            if (variableValue.ToString() == "")
            {
                
                Globalvariables.Add(variableName, new[] { dataType, variableValue?.ToString()});
            }
            else
            {
                StoreVariable(variableName,dataType,varCtx.expression().GetText(), variableValue?.ToString());
            }

        }
        return null;
    }

    public override object? VisitAssignment(SpeakParser.AssignmentContext context)
    {
        //The last child is always an expression
        int lastIndex = context.ChildCount - 1;
        var lastChild = context.GetChild(lastIndex);
       
        var expression = lastChild.GetText();
        var value = Visit(lastChild);
      
       
        var variables = context.GetText().Split('=');
        bool isMatch_variableName = false;
        //check the variable
        //iterate all except the last one because it is the value
        for (int i = 0; i < variables.Length -1 ; i++)
        {
            //if either is true then invalid variable
            if (!CheckVariables(variables[i]) || _reservedKeywords.Contains(variables[i]))
            {
             
                throw new Exception("Invalid Variable");
            }
            else
            {
                var variableInfo = Globalvariables[variables[i]];
                AssignVariable(variables[i],variableInfo[0],expression, value.ToString());
            }
        }



        return base.VisitAssignment(context);
    }

    public override object? VisitDisplay(SpeakParser.DisplayContext context)
    {
        var str = new StringBuilder();
        var value = Visit(context.print());
        str.Append(value.ToString());
       
        Console.WriteLine(str.ToString());
        return null!;
    }
    

    public override object? VisitPrintIdentifier(SpeakParser.PrintIdentifierContext context)
    {
        var variableName = context.IDENTIFIER().GetText();
        //check if variableName is a keyword
        if (_reservedKeywords.Contains(variableName))
            throw new Exception("Variable name is a keyword");
        
        if (CheckVariables(variableName)) // It is a variable
        {
            string[] varInfo = Globalvariables[variableName];
            if (varInfo[1] == "")
            {
                throw new Exception($"Variable name'{variableName}' has no define value");
            }

            return varInfo[1];
            
        }
        else // It is a variable that are not define
        {
                throw new Exception($"Variable {variableName} undefined.");
        }
          
    }

    public override object? VisitPrintConcat(SpeakParser.PrintConcatContext context)
    {
        var left = Visit(context.print(0))!;
        var right = Visit(context.print(1))!;
        return left.ToString() + right.ToString();
    }

    public override object? VisitPrintNewline(SpeakParser.PrintNewlineContext context)
    {
        return "\n";
    }

    public override object? VisitPrintEscape(SpeakParser.PrintEscapeContext context)
    {
        var value = context.ESCAPE().GetText();
        value = value.Remove(0, 1); // Remove [
        value = value.Remove(value.Length-1, value.Length-1); // Remove ]
        return value;
    }

    public override object? VisitPrintString(SpeakParser.PrintStringContext context)
    {
        var value = context.STRING().GetText();
        value = value.Remove(0, 1); // Remove "
        value = value.Replace("\"", ""); // Remove "
        return value;
    }

    public override object VisitConstant(SpeakParser.ConstantContext context)
    {
       
        if (context.INTEGER() is { } i)
            return int.Parse(i.GetText());
        
        if (context.FLOAT() is { } f)
            return float.Parse(f.GetText());
        
        if (context.CHAR() is { } c)
            return Convert.ToChar(c.GetText().Replace("\'", ""));
        
        if (context.BOOL() is { } b)
            return Convert.ToBoolean(b.GetText().Replace("\"", ""));
    

        throw new NotImplementedException();
    }

    public override object? VisitIdentifierExpression(SpeakParser.IdentifierExpressionContext context)
    {

        string variableName = context.IDENTIFIER().GetText();
        if (Globalvariables.ContainsKey(variableName))
        {
            var varInfo = Globalvariables[variableName];
            return varInfo[1];
        }
        else
        {
            throw new Exception($"Variable {variableName} does not exist!");
        }

      
    }

    public override object? VisitParenthesizedExpression(SpeakParser.ParenthesizedExpressionContext context)
    {
        return Visit(context.expression())!;
       
    }

    public override object? VisitUnaryExpression(SpeakParser.UnaryExpressionContext context)
    {
        return Convert.ToDouble(context.GetText());
    }


    public override object? VisitFirstPrecedence(SpeakParser.FirstPrecedenceContext context)
    {
        var left = Visit(context.expression(0))!;
        var right = Visit(context.expression(1))!;
        
        if (left.ToString() == "" || right.ToString() == "")
        {
            throw new InvalidOperationException("Cannot operate if value is empty");
        }

        switch (context.firstOp().GetText())
        {
            case "*":
                return (Convert.ToDouble(left) * Convert.ToDouble(right)).ToString("N1");
            case "/":
                return (Convert.ToDouble(left) / Convert.ToDouble(right)).ToString("N1");
            case "%":
                return (Convert.ToDouble(left) % Convert.ToDouble(right)).ToString("N1");
            default:
                throw new InvalidOperationException("Invalid operator");
        }
    }

    public override object? VisitSecondPrecedence(SpeakParser.SecondPrecedenceContext context)
    {
       
        var left = Visit(context.expression(0))!;
        var right = Visit(context.expression(1))!;

       
        switch (context.secondOp().GetText())
        {
            
            case "+":
                return (Convert.ToDouble(left) + Convert.ToDouble(right)).ToString("N1");
            case "-":
                return (Convert.ToDouble(left) - Convert.ToDouble(right)).ToString("N1");
            default:
                throw new InvalidOperationException("Invalid operator");
        }
    }

    public override object? VisitComparisonExpression(SpeakParser.ComparisonExpressionContext context)
    {
        var left = Visit(context.expression(0))!;
        var right = Visit(context.expression(1))!;
 
        switch (context.comparison_operators().GetText())
        {
            case ">":
                return Convert.ToDouble(left) > Convert.ToDouble(right);
            case "<":
                return Convert.ToDouble(left) < Convert.ToDouble(right);
            case ">=":
                return Convert.ToDouble(left) >= Convert.ToDouble(right);
            case "<=":
                return Convert.ToDouble(left) <= Convert.ToDouble(right);
            case "==":
                return Convert.ToDouble(left) == Convert.ToDouble(right);
            case "<>":
                return Convert.ToDouble(left) != Convert.ToDouble(right);
            default:
                throw new InvalidOperationException("Invalid operator");
        }
    }

    public override object? VisitLogicalExpression(SpeakParser.LogicalExpressionContext context)
    {
        var left = Visit(context.expression(0))!;
        var right = Visit(context.expression(1))!;
        switch (context.locical_operators().GetText())
        {
            case "AND":
                return (bool)left && (bool)right;
            case "OR":
                return (bool)left || (bool)right;
            default:
                throw new InvalidOperationException("Invalid operator");
        }
    }

    public override object? VisitNotExpression(SpeakParser.NotExpressionContext context)
    {
        var expression = Visit(context.expression())!;
        switch (context.NOT().GetText())
        {
            case "!":
            {
                if (expression is bool)
                {
                    return !(bool)expression;
                } else
                {
                    throw new Exception("Invalid boolean value");
                }
            }
            default:
                throw new InvalidOperationException("Invalid operator");
        }
    }
}