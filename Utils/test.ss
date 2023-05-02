BEGIN CODE
INT x = 5
IF (x == 5) 
BEGIN IF
    DISPLAY:  "x is equal to 5."
    INT y = 10
    IF (y == 10) BEGIN IF
        DISPLAY: "y is equal to 10."
    END If 
    ELSE 
    BEGIN IF
        DISPLAY: "y is not equal to 10."
    END IF
END IF 
ELSE BEGIN IF
    DISPLAY: "x is not equal to 5."
END IF
#this will error because y is declared inside the if statement
 DISPLAY: y
END CODE
