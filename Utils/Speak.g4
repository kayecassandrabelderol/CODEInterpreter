grammar Speak;

program: 'BEGIN CODE' line* 'END CODE';

line: (declaration | comment) NEWLINE*;

declaration: dataType IDENTIFIER;

expression: INT | CHAR;

comment: COMMENT;
    
dataType: 'INT' | 'FLOAT' | 'CHAR' | 'BOOL';

COMMENT: '#' ~[\r\n]* (' ' ~[\r\n]*)* ;

INT: [0-9]+;
FLOAT: [0-9]+ '.' [0-9]+;
CHAR: [a-zA-Z];
BOOL: 'true' | 'false';

WS: [ \t\r\n]+ -> skip;
NEWLINE: [\r\n];
IDENTIFIER: ([a-zA-Z_]([a-zA-Z0-9_])*);