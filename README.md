## log-exp-evaluator

A logical expression parser and evaluator implemented using [reverse Polish notation](https://en.wikipedia.org/wiki/Reverse_Polish_notation).

An input data consists of logical expressions, separated with ";".  
Logical expressions consist of:
* variables' identifiers;
* constants ("0", "1");
* assignment sign (":=")
* operations ("or", "xor", "and", "not");
* parenthesis ("(", ")").

Warning: the code is not refactored and is very creepy, but it works quite correctly. This was kinda legacy that I was given by a person who didn't know about what "code smell" is, but he did a good enough implementation of RPN, but with ariphmetic operators, so I changed it according to my task requirements.  
I do not write code this way, but it might be helpful for those looking for a RPN implementation in C#.

## Screenshots

![Screenshot 1](https://drive.google.com/uc?id=1pLNihnw5lV2iofd3K3GT7gmHYu33ZM1k)


