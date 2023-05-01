grammar Speak;

program: comment* 'BEGIN CODE' line* 'END CODE' comment* EOF;

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

display: 'DISPLAY:' print;


scan: 'SCAN:' IDENTIFIER (',' IDENTIFIER)*;

print :     STRING						     #printString
			| IDENTIFIER					 #printIdentifier
			| '$'							 #printNewline
			| ESCAPE						 #printEscape
			| print '&' print	             #printConcat	
			;

ESCAPE: '[' . ']' ;
CONCATENATE: '&';
LINEBREAK: '$'; 

COMMENT: '#' ~[\r\n]* (' ' ~[\r\n]*)* ;


locical_operators: AND | OR;

bool_expression
                   : BOOL
                   | expression locical_operators expression
                   | expression comparison_operators expression
                   | NOT bool_expression;
                
//statement: nested_statement | expression;
//nested_statement: 'ELSE' statement* | 'ELSE IF' statement*;
//conditional: if_selection | if_else_selection; //| if_else_multiple;

//if_selection: 'IF' '(' bool_expression ')' 'BEGIN IF' line* statement* 'END IF';
//if_else_selection: 'IF' '(' bool_expression ')' 'BEGIN IF' line* expression* 'END IF' line* 'ELSE' 'BEGIN IF' line* expression* 'END IF';
//if_else_multiple: 'IF' '(' expression ')' nested else_if_selection* 'ELSE' nested;
//else_if_selection: 'ELSE IF' '(' expression ')' nested;

//operators

constant: (INTEGER | FLOAT) | BOOL | CHAR ;
//Data types
INTEGER:  [0-9]+;
FLOAT:  (([0-9]+ '.' [0-9]+) | ([0-9]? '.' [0-9]+));
CHAR: '\'' [a-zA-Z] '\'';
BOOL: '"' 'TRUE' '"'  | '"' 'FALSE' '"';
STRING: '"' ~'"'* '"';
ADD: '+';
SUB: '-';
//Arithmetic
firstOp: '*' | '/' | '%';
secondOp: '+' | '-' ;

//Comparison operators
comparison_operators: '>' | '<' | '>=' | '<=' | '==' | '<>';

//Logical
AND: 'AND';
OR: 'OR';
NOT: '!';

WS: [ \t\r\n]+ -> skip;
NEWLINE: [\r\n];
IDENTIFIER: [a-zA-Z_] [a-zA-Z0-9_]*;

