grammar CODE;

program: comment* line* comment*;

line: (declaration | declaration assignment | assignment | declareassign | conditional | display | scan | comment | statement) NEWLINE* comment*;

declaration: ( (dataType IDENTIFIER) | (dataType IDENTIFIER(',' IDENTIFIER)*) );

assignment: ( (IDENTIFIER('=' expression) ('=' expression)*) | (IDENTIFIER('=' expression)) | (IDENTIFIER('=' expression) (expression)*) );

declareassign: ( (dataType IDENTIFIER('=' expression) ((',' IDENTIFIER('=' expression)?)*) ((',' IDENTIFIER)?'='? expression)*) | (dataType IDENTIFIER(',' IDENTIFIER('=' expression)?)* ((',' IDENTIFIER)?'='? expression)*) );

expression 
    : INT 
    | CHAR 
    | FLOAT 
    | (dataType)? IDENTIFIER 
    | ADD 
    | SUBTRACT 
    | MULTIPLY 
    | DIVIDE 
    | MODULO 
    | GREATER_THAN 
    | LESS_THAN 
    | GREATER_THAN_EQUAL_TO 
    | LESS_THAN_EQUAL_TO 
    | EQUAL 
    | NOT_EQUAL;
    
comment: COMMENT;

dataType: 'INT' | 'FLOAT' | 'CHAR' | 'BOOL';

display: 'DISPLAY:' print ('&' print)*;
scan: 'SCAN:' read (',' read)*;
print : IDENTIFIER | STRING | expression;
read: IDENTIFIER | STRING;

COMMENT: '#' ~[\r\n]* (' ' ~[\r\n]*)* ;

INT: ([0-9]+ | '-' [0-9]+);
FLOAT: (([0-9]+ '.' [0-9]+) |'-' ([0-9]+ '.' [0-9]+) | ([0-9]? '.' [0-9]+) | '-' ([0-9]? '.' [0-9]+));
CHAR: '\'' [a-zA-Z] '\'';
BOOL: '"' 'TRUE' '"'  | '"' 'FALSE' '"';
STRING: '"' ~'"'* '"';

ADD: ('+');
SUBTRACT: ('-');
MULTIPLY: ('*');
DIVIDE: ('/');
MODULO: ('%');

locical_operators: GREATER_THAN | LESS_THAN | GREATER_THAN_EQUAL_TO | LESS_THAN_EQUAL_TO | EQUAL | NOT_EQUAL;
bool_expression: expression locical_operators expression;
statement: nested_statement | expression;
nested_statement: 'ELSE' statement* | 'ELSE IF' statement*;
conditional: if_selection | if_else_selection; //| if_else_multiple;

if_selection: 'IF' '(' bool_expression ')' 'BEGIN IF' line* statement* 'END IF';
if_else_selection: 'IF' '(' bool_expression ')' 'BEGIN IF' line* expression* 'END IF' line* 'ELSE' 'BEGIN IF' line* expression* 'END IF';
//if_else_multiple: 'IF' '(' expression ')' nested else_if_selection* 'ELSE' nested;
//else_if_selection: 'ELSE IF' '(' expression ')' nested;

GREATER_THAN: '>';
LESS_THAN: '<';
GREATER_THAN_EQUAL_TO: '>=';
LESS_THAN_EQUAL_TO: '<=';
EQUAL: '==';
NOT_EQUAL: '<>';

WS: [ \t\r\n]+ -> skip;
NEWLINE: [\r\n];
IDENTIFIER: [a-zA-Z_] [a-zA-Z0-9_]*;
