grammar Speak;

program: comment* 'BEGIN CODE' line* 'END CODE' comment*;

line: (declaration assignment? | declaration assignment | declareassign | display | scan | comment) NEWLINE* comment*;

// with multiple declaration
declaration: ((dataType IDENTIFIER) | (dataType IDENTIFIER(',' IDENTIFIER)*));
// multiple assignment with or without data type
assignment: ((IDENTIFIER('=' expression)('=' expression)*) | (IDENTIFIER('=' expression)));
// declaration and initialization (=, ',')
declareassign: ((dataType IDENTIFIER('=' expression) (',' IDENTIFIER('=' expression)?)*) | (dataType IDENTIFIER(',' IDENTIFIER('=' expression)?)*));

expression: INT | CHAR | FLOAT | (dataType)? IDENTIFIER | ADD | SUBTRACT | MULTIPLY | DIVIDE;
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
DIVIDE: ('/' | '%');

WS: [ \t\r\n]+ -> skip;
NEWLINE: [\r\n];
IDENTIFIER: [a-zA-Z_] [a-zA-Z0-9_]*;
