using Antlr4.Runtime;
using CODEInterpreter_v1;
using CODEInterpreter_v1.Utils;

var fileName = @"C:\Users\Hardelle\Downloads\CODEInterpreter-Assignment-JB\Utils\test.ss";


if (!File.Exists(fileName))
{
    Console.WriteLine($"Error: Could not find file {fileName}");
    return;
}

var fileContent = File.ReadAllText(fileName);
var input = new AntlrInputStream(fileContent);
var lexer = new SpeakLexer(input);
var tokens = new CommonTokenStream(lexer);
var parser = new SpeakParser(tokens);

//custom error listener
parser.RemoveErrorListeners(); // Remove default console error listener
parser.AddErrorListener(new CustomErrorListener());

try
{
    // Start parsing
    var tree = parser.program();
    // Process the parsed tree here
    var visitor = new SpeakVisitor();
    visitor.Visit(tree);
}
catch (RecognitionException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}