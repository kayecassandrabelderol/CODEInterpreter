grammar Speak;

program: comment* 'BEGIN CODE' line* 'END CODE' comment*;

line: (declaration |assignment| display |comment) NEWLINE* comment*;



// with multiple declaration
declaration: dataType IDENTIFIER (',' IDENTIFIER)*;
// multiple assignment with or without data type
assignment:  ((dataType IDENTIFIER) | IDENTIFIER) ('=' expression | ('=' IDENTIFIER)+) (',' ((dataType IDENTIFIER) | IDENTIFIER) ('=' expression | ('=' IDENTIFIER)+)*)*;

expression: INT | CHAR | FLOAT | IDENTIFIER; 

comment: COMMENT;
    
dataType: 'INT' | 'FLOAT' | 'CHAR' | 'BOOL';

display: 'DISPLAY:' print ('&' print)*;
print : IDENTIFIER |  STRING | expression;
COMMENT: '#' ~[\r\n]* (' ' ~[\r\n]*)* ;

INT: [0-9]+;
FLOAT: [0-9]+ '.' [0-9]+;
CHAR: '\'' [a-zA-Z] '\'';
BOOL: '"' 'TRUE' '"'  | '"' 'FALSE' '"';

STRING: '"' ~'"'* '"';

WS: [ \t\r\n]+ -> skip;
NEWLINE: [\r\n];
IDENTIFIER: [a-zA-Z_] [a-zA-Z0-9_]*;
