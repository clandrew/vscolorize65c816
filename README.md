# VSColorize65C816
This is a Visual Studio extension providing [64tass assembler-](http://tass64.sourceforge.net)compatible syntax highlighting for W65C816. This extension was created for personal convenience.

![Example image](https://raw.githubusercontent.com/clandrew/vscolorize65c816/main/images/example0.png "Example image")

Language elements colorized:
* Mnemonics are highlighted as keywords
* Comments
* Strings
* Assembler directives

## Build
This extension was written in C# using Visual Studio 2019 Community, tested on the same. It is built for Windows platform x86-64 architecture. To build. open the .sln solution file and choose 'Build' in Visual Studio.

## Notes
* The opcode "INC" is not highlighted, because 64tass's convention is "INA".
* Some alternate mnemonics in 64tass's documentation such as CLP and CSP are recognized.
