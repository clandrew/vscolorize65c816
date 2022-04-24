# VSColorize65C816
This is a Visual Studio extension providing [64tass assembler-](http://tass64.sourceforge.net)compatible syntax highlighting for W65C816. This extension was created for personal convenience.

![Example image](https://raw.githubusercontent.com/clandrew/vscolorize65c816/main/images/example0.png "Example image")

It  doesn't exhaustively try to highlight operators or addressing mode-related punctuation- that was a non-goal. Instead, the aim was simplicity and ease of readability. 
Language elements colorized:
* Mnemonics are highlighted as keywords
* Comments
* Strings
* Assembler directives

Tested on all the [samples](https://github.com/pweingar/C256Samples) available.

## Build
This extension was written in C# using Visual Studio 2019 Community, tested on the same. It is built for Windows platform x86-64 architecture. To build. open the .sln solution file and choose 'Build' in Visual Studio.

## Notes
* The opcode "INC" is not highlighted, because 64tass's convention is "INA".
* Some alternate mnemonics in 64tass's documentation such as CLP and CSP are recognized.
