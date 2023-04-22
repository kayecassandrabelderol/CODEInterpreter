grammar Speak;

program: comment* 'BEGIN CODE' line* 'END CODE' comment*;

line: (declaration | display | scan | assignment |comment) NEWLINE* comment*;

declaration:  dataType variable (',' variable)*;
//count the number of expression if only 1 then the rest is identifier
//if more 2 then it is error
assignment:  IDENTIFIER ('=' IDENTIFIER)* '=' expression;
variable: IDENTIFIER ('=' expression)?;

        
              
expression
    : IDENTIFIER                                          #identifierExpression
    | '(' expression ')'                                  #parenthesizedExpression
    | NOT expression                                      #notExpression
    | expression firstOp expression                       #firstPrecedence
    | expression secondOp expression                      #secondPrecedence
    | expression comparison_operators expression          #comparisonExpression
    | expression locical_operators expression             #logicalExpression
    | constant                                            #constantExpression
    | (ADD | SUB) expression                              #unaryExpression
    ;

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