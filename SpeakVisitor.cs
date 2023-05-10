using Antlr4.Runtime.Misc;
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

    public void StoreVariable(string variableName, string dataType, string expression, string value, string type)
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
                if (type == "ADD")
                {
                    Globalvariables.Add(variableName, new[] { dataType, value });
                }
                else if(type == "ASSIGN")
                {
                    Globalvariables[variableName] = new[] { dataType, value };
                }
                else
                {
                    throw new Exception("Type of storing is not recognized");
                }
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

    public void AssignVariableScan(string variableName, string dataType, string value)
    {
        bool valid = CheckValue(dataType, value);
       
        if (valid)
        {
            Globalvariables[variableName] = new[] { dataType, value };
        }
        else
        {
            throw new Exception("Invalid Value" + value);
        }
        
    }

    public override object? VisitDeclaration(SpeakParser.DeclarationContext context)
    {
        string dataType = context.dataType().GetText();
        List<string> variablesAdded = new List<string>();
        foreach (SpeakParser.VariableContext varCtx in context.variable())
        {

            string variableName = varCtx.IDENTIFIER().GetText();

            // Check if variable already exists
            if (Globalvariables.ContainsKey(variableName))
            {
                throw new Exception($"Variable '{variableName}' already exists!");
            }

            variablesAdded.Add(variableName);

            //check if variable is a keyword
            if (_reservedKeywords.Contains(variableName))
                throw new Exception($"Variable '{variableName}' is a keyword");
            
            var variableValue = varCtx.expression() != null ? Visit(varCtx.expression()) : "";
            
            //if variable value is empty or just initialization
            if (variableValue.ToString() == "")
            {
                Globalvariables.Add(variableName, new[] { dataType, variableValue?.ToString() });
            }
            else
            {
                StoreVariable(variableName, dataType, varCtx.expression().GetText(), variableValue?.ToString(), "ADD");
            }

        }
        //we can return an array here for the if statement to count
        return variablesAdded;
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
                StoreVariable(variables[i],variableInfo[0],expression, value.ToString(),"ASSIGN");
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
        // Check if variableName is a keyword
        if (_reservedKeywords.Contains(variableName))
            throw new Exception($"Variable '{variableName}' is a keyword");
        
        try
        {
            if (CheckVariables(variableName)) // It is a variable
            {
                string[] varInfo = Globalvariables[variableName];
                if (varInfo[1] == "")
                {
                    throw new Exception($"Variable name '{variableName}' has no defined value");
                }

                return varInfo[1];
            }
            else // It is a variable that is not defined
            {
                throw new Exception($"Variable '{variableName}' is undefined.");
            }
        }
        catch (Exception ex)
        {
            // Display the exception message instead of the line of code
            Console.WriteLine("Exception occurred: " + ex.Message);
            Environment.Exit(1);
            return null;
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

    public override object? VisitPrintExpression(SpeakParser.PrintExpressionContext context)
    {
        return Visit(context.expression());
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
        try
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
        catch (Exception ex)
        {
            // Display the exception message instead of the line of code
            Console.WriteLine("Exception occurred: " + ex.Message);
            Environment.Exit(1);
            return null;
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

        var op = context.firstOp().GetText();
        switch (op)
        {
            case "*":
                return (Convert.ToDouble(left) * Convert.ToDouble(right)).ToString("N1");
            case "/":
                return (Convert.ToDouble(left) / Convert.ToDouble(right)).ToString("N1");
            case "%":
                return (Convert.ToDouble(left) % Convert.ToDouble(right)).ToString("N1");
            default:
                //cannot perform this error if wala gi declare sa grammar ang any other symbols
                throw new InvalidOperationException($"Invalid operator '{op}'");
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
        var left = Visit(context.expression(0))!.ToString();
        var right = Visit(context.expression(1))!.ToString();

 
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
                 return left.Equals(right);
            case "<>":
                return !left.Equals(right);
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
        //changes
        var booleanValue = expression.ToString();
                if (booleanValue == "True" || booleanValue == "False")
                {
                    if (booleanValue == "True")
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                } else
                {
                    throw new Exception($"Invalid boolean value '{booleanValue}'");
                }

    }


    public string isConstant(string value)
    {
       string numConstant =@"^[-+]?(\d+(\.\d*)?|\.\d+)$"  ;
       string patternChar  = @"^'([a-zA-Z])'$";
       string patternBool = @"""TRUE""|""FALSE""";
       value = value.Trim();
       // not int
       
       if (Regex.IsMatch(value.Trim(), numConstant))
       {
           return value;
       }
       else if (Regex.IsMatch(value.Trim(), patternChar))
       {
           return value.Replace("\'", "");
       }
       else if (Regex.IsMatch(value.Trim(), patternBool))
       {
           
            value = value.Replace("\"", "").ToLower();
            return char.ToUpper(value[0]) + value.Substring(1);
       }
       else
       {
           throw new Exception("Invalid constant");
           
       }

       return value;

    }

    public override object VisitScan([NotNull] SpeakParser.ScanContext context)
    {
        var identifiers = context.IDENTIFIER(); // Get all identifier tokens
     
        var value = Console.ReadLine();
        string sValue = value;
        var values = sValue.Split(','); //not working
       
        
        if (!(identifiers.Length == values.Length))
        {
            throw new Exception("Out of Bounds");
        }

        for (int i = 0; i < values.Length; i++)
        {
           
            values[i] = isConstant( values[i]);
           
        }

        int num = 0;
        foreach (var identifier in identifiers)
        {
            //to ask
            if (!CheckVariables(identifier.GetText()) || _reservedKeywords.Contains(identifier.GetText()))
            {
                //to ask
                throw new Exception($"Invalid variable");
            }
            else
            {
                var variableInfo = Globalvariables[identifier.GetText()];
                
                AssignVariableScan(identifier.GetText(),variableInfo[0], values[num]);
            }

            num++;
        }
        return base.VisitScan(context);
        
    }

    public override object? VisitConditional_if_statement(SpeakParser.Conditional_if_statementContext context)
    {
        SpeakParser.If_statementContext ifStatement = context.if_statement();
        
        //evaluate the expression
        object value = Visit(ifStatement.expression());
        //if true return
        //if false ignore and move into another condition
        if (value.ToString() == "True")
        {
            return Visit(ifStatement);
        }

        //same logic above
        foreach (SpeakParser.Else_if_statementContext varCtx in context.else_if_statement())
        {
            value = Visit(varCtx.expression());
          
            if (value.ToString() == "True")
            {
                return Visit(varCtx);
            }
        }
        
        //if above condition remains false
        //implement the statement of the else
        SpeakParser.Else_statementContext elseStatement = context.else_statement();
        if (elseStatement != null) 
        {
        return Visit(elseStatement);
        }
        
        return null;
       
    }

    public override object? VisitIf_statement(SpeakParser.If_statementContext context)
    { 
        List<string> variablesAdded = new List<string>();
        List<string> localVariables = new List<string>();
        foreach (var child in context.children)
        {
            
            if (child is SpeakParser.LineContext lineContext)
            {
                foreach (var lineChild in lineContext.children)
                {
                    if (lineChild is SpeakParser.DeclarationContext declarationContext)
                    {
                        Object variables = VisitDeclaration(declarationContext);
                        variablesAdded = variables as List<string>;
                        localVariables.AddRange(variablesAdded);
                    }
                    else
                    {
                        Visit(lineChild);
                    }
                }
            }
            else
            {
                Visit(child);
            }
        }

        foreach (string variable in variablesAdded)
        {
           
            Globalvariables.Remove(variable);
        }
      
        
        return null;
    }

    public override object? VisitElse_if_statement(SpeakParser.Else_if_statementContext context)
    {
    
        List<string> variablesAdded = new List<string>();
        List<string> localVariables = new List<string>();
        foreach (var child in context.children)
        {
            
            if (child is SpeakParser.LineContext lineContext)
            {
                foreach (var lineChild in lineContext.children)
                {
                    if (lineChild is SpeakParser.DeclarationContext declarationContext)
                    {
                        Object variables = VisitDeclaration(declarationContext);
                        variablesAdded = variables as List<string>;
                        localVariables.AddRange(variablesAdded);
                    }
                    else
                    {
                        Visit(lineChild);
                    }
                }
            }
            else
            {
                Visit(child);
            }
        }

        foreach (string variable in variablesAdded)
        {
           
            Globalvariables.Remove(variable);
        }
      
        
        return null;
    }

    public override object? VisitElse_statement(SpeakParser.Else_statementContext context)
    {
        List<string> variablesAdded = new List<string>();
        List<string> localVariables = new List<string>();
        foreach (var child in context.children)
        {
            
            if (child is SpeakParser.LineContext lineContext)
            {
                foreach (var lineChild in lineContext.children)
                {
                    if (lineChild is SpeakParser.DeclarationContext declarationContext)
                    {
                        Object variables = VisitDeclaration(declarationContext);
                        variablesAdded = variables as List<string>;
                        localVariables.AddRange(variablesAdded);
                    }
                    else
                    {
                        Visit(lineChild);
                    }
                }
            }
            else
            {
                Visit(child);
            }
        }

        foreach (string variable in variablesAdded)
        {
           
            Globalvariables.Remove(variable);
        }
        return null;
    }

    public override object? VisitWhile_statement(SpeakParser.While_statementContext context)
    {
        object expression = Visit(context.expression());
       
        List<string> variablesAdded = new List<string>();
        List<string> localVariables = new List<string>();
        if (expression.ToString() == "True")
        {
            bool value = true;
            while (value)
            {
                foreach (var child in context.children)
                {
                    if (child is SpeakParser.LineContext lineContext)
                    {
                        foreach (var lineChild in lineContext.children)
                        {
                            if (lineChild is SpeakParser.DeclarationContext declarationContext)
                            {
                                Object variables = VisitDeclaration(declarationContext);
                                variablesAdded = variables as List<string>;
                                localVariables.AddRange(variablesAdded);
                            }
                            else
                            {
                                Visit(lineChild);
                            }
                        }
                    }
                    else
                    {
                        Visit(child);
                    }
                    
                }
                
                expression = Visit(context.expression());
                //if false exit the while statement and clear the varialbes inside
                if (expression.ToString() == "False")
                {
                    foreach (string variable in variablesAdded)
                    {
                   
                        Globalvariables.Remove(variable);
                    }
                    return null;
                }
                //every time it iterates all declared variables will be remove
                foreach (string variable in variablesAdded)
                {
                   
                    Globalvariables.Remove(variable);
                }
                
            }
        }
        else if (expression.ToString() == "False")
        {
            return null;
        }
        else
        {
            throw new Exception("The expression value in While statement is not Boolean");
        }
        return null;
    }
    
    
}