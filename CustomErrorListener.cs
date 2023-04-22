using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
namespace CODEInterpreter_v1;

public class CustomErrorListener : IAntlrErrorListener<IToken>
{
    public void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
    {
        Console.WriteLine($"Syntax Error at line {line}, position {charPositionInLine}: {msg}");
    }
}