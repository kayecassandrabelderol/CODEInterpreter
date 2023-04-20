using Antlr4.Runtime;
using CODEInterpreter_v1;
using CODEInterpreter_v1.Utils;
using static CODEInterpreter_v1.Utils.CODEParser;

var fileName = "C:\\Belderol\\3rd Year\\Second Semester\\CS322 Programming Languages\\Activities\\CODEInterpreter\\Utils\\test.code";
var fileContents = File.ReadAllText(fileName);
var inputStream = new AntlrInputStream(fileContents);
var codeLexer = new CODELexer(inputStream);
var commonTokenStream = new CommonTokenStream(codeLexer);
var codeParser = new CODEParser(commonTokenStream);
var codeContext = codeParser.program();
var visitor = new CODEVisitor();
visitor.Visit(codeContext);