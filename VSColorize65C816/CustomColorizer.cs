using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Runtime.InteropServices;

namespace VSColorize65C816
{
    class CustomColorizer : IVsColorizer
    {
        private enum Colors : uint
        {
            Normal_Black = 1,
            Keyword_Blue = 2,
            Comment_Green = 3,
            Preprocessor_Gray = 4,
            Red_String = 5,
        }

        public CustomColorizer(IVsTextLines pBuffer)
        {
        }

        int IVsColorizer.GetStartState(out int startState)
        {
            startState = 0;
            return VSConstants.S_OK;
        }

        int IVsColorizer.GetStateMaintenanceFlag(out int flag)
        {
            flag = 0; // We don't require per-line state maintenance, because this language doesn't have mutli-line comments or so forth.
            return VSConstants.S_OK;
        }

        class Parser
        {
            public int index = 0;
            public string text;
            public uint[] attributes;
            int length;


            enum State
            {
                None,
                Comment,
                DirectiveWithNoArg,
                DirectiveWithStringArg,
            }
            State s = State.None;

            public Parser(int l, IntPtr pszText, uint[] a)
            {
                text = Marshal.PtrToStringUni(pszText, l);
                attributes = a;
                length = l;
            }

            bool IsLetter(char c)
            {
                return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
            }

            bool IsLabelStartingChar(char c)
            {
                return c == '_' || IsLetter(c);
            }

            public void EatLabel()
            {
                if (!IsLabelStartingChar(text[0]))
                    return;

                // Look for label at the beginning. Labels are just normal color
                int j = 1;
                while (j < text.Length && IsCharacter(text[j]))
                {
                    ++j;
                }
                index = j;
            }

            void DoComment()
            {
                for (int j = index; j < text.Length; ++j)
                {
                    attributes[j] = (uint)Colors.Comment_Green;
                }
            }

            bool HasDirective(string directiveName)
            {
                if (index >= length - directiveName.Length)
                    return false;

                for (int i = 0; i < directiveName.Length; ++i)
                {
                    if (text[index + 1 + i] != directiveName[i])
                        return false;
                }

                if (!IsWhitespaceOrDelimiterAt(index + directiveName.Length + 1))
                    return false;

                return true;
            }

            bool HasSomeDirective(out int directiveLength)
            {
                for (int i = 0; i < allDirectivesWithNoStringArg.Length; ++i)
                {
                    if (HasDirective(allDirectivesWithNoStringArg[i]))
                    {
                        directiveLength = allDirectivesWithNoStringArg[i].Length;
                        return true;
                    }
                }

                directiveLength = -1;
                return false;
            }

            void DoDirective(int directiveLength)
            {
                for (int i = 0; i < directiveLength + 1; ++i)
                {
                    attributes[index + i] = (uint)Colors.Preprocessor_Gray;
                }
                index += directiveLength + 1;
            }

            readonly string[] allOpcodes = { 
                // Standard opcodes
                "ADC", "AND", "ASL",
                "BCC", "BCS", "BEQ", "BIT", "BMI", "BNE", "BPL", "BRA", "BRK", "BRL", "BVC", "BVS",
                "CLC", "CLD", "CLI", "CLV", "CMP", "COP", "CPX", "CPY",
                "DEC", "DEX", "DEY",
                "EOR",
                "INA", "INX", "INY",
                "JML", "JMP", "JSL", "JSR",
                "LDA", "LDX", "LDY", "LSR",
                "MVN", "MVP",
                "NOP",
                "ORA",
                "PEA", "PEI", "PER", "PHA", "PHB", "PHD", "PHK", "PHP", "PHX", "PHY", "PLA", "PLB", "PLD", "PLP", "PLX", "PLY",
                "REP", "ROL", "ROR", "RTI", "RTL", "RTS",
                "SBC", "SEC", "SED", "SEI", "SEP", "STA", "STP", "STX", "STY", "STZ",
                "TAX", "TAY", "TCD", "TCS", "TDC", "TRB", "TSB", "TSC", "TSX", "TXA", "TXS", "TXY", "TYA", "TYX",
                "WAI", "WDM",
                "XBA", "XCE", 

                // 'Additional aliases' from the 64tass manual
                "CLP", "CPA", "CSP", "HLT", "JML", "SWA", "TAD", "TAS", "TDA", "TSA"};

            readonly string[] allDirectivesWithNoStringArg =
            {
                "addr", "al", "align", "as", "assert", "autsiz",
                "bend", "binary", "binclude", "bfor", "block", "break", "breakif", "brept", "bwhile", "byte",
                "case", "cdef", "cerror", "char", "check", "comment", "continue", "continueif", /*"cpu",*/ "cwarn",
                "databank", "default", "dint", "dpage", "dsection", "dstruct", "dunion", "dword",
                "edef", "else", "elsif", "enc", "end", "endblock", "endc", "endcomment", "endf", "endfor", "endfunction", "endif", "endlogical",
                "endm", "endmacro", "endn", "endnamespace", "endp", "endpage", "endproc", "endrept", "ends", "endsection", "endsegment", "endstruct",
                "endswitch", "endu", "endunion", "endv", "endvirtual", "endweak", "endwhile", "endwith", "eor", "error",
                "fi", "fill", "for", "function",
                "goto",
                "here", "hidemac",
                "if", "ifeq", "ifmi", "ifne", "ifpl", /*"include",*/
                "lbl", "lint", "logical", "long",
                "macro", "mansiz",
                "namespace", "next", "null",
                "offs", "option",
                "page", "pend", "proc", "proff", "pron", "ptext",
                "rept", "rta",
                "section", "seed", "segment", "send", "sfunction", "shift", "shiftl", "showmac", "sint", "struct", "switch",
                "text",
                "union",
                "var", "virtual",
                "warn", "weak", "while", "with", "word",
                "xl", "xs"

            };


            public void ProcessLine()
            {
                int directiveLength;

                while (index < text.Length)
                {
                    if (text[index] == ';')
                    {
                        DoComment();
                        return;
                    }
                    else if (text[index] == '.')
                    {
                        if (HasDirective("cpu"))
                        {
                            DoDirective(3); // .cpu "65816"
                            s = State.DirectiveWithStringArg;

                        }
                        else if (HasDirective("include"))
                        {
                            DoDirective(7); // .include whatever
                            s = State.DirectiveWithStringArg;
                        }
                        else if (index < length - 2 && (text[index + 1] == 'a' || text[index + 1] == 'x' || text[index + 1] == 'y') && (text[index + 2] == 's' || text[index + 2] == 'l') && IsWhitespaceOrDelimiterAt(index + 3))
                        {
                            // .as, .xl, and so on
                            attributes[index + 0] = (uint)Colors.Preprocessor_Gray;
                            attributes[index + 1] = (uint)Colors.Preprocessor_Gray;
                            attributes[index + 2] = (uint)Colors.Preprocessor_Gray;
                            index += 3;
                        }
                        else if (HasSomeDirective(out directiveLength))
                        {
                            DoDirective(directiveLength);
                        }
                        else
                        {
                            // Period followed by something unrecognized
                            ++index;
                        }
                    }
                    else if (text[index] == '\"')
                    {
                        // Strings are enclosed with ". Literal quotes are represented as two "", so we don't need to do anything special to handle escaped literal quotes.
                        uint color = s == State.DirectiveWithStringArg ? (uint)Colors.Preprocessor_Gray : (uint)Colors.Red_String;

                        // Grey or red out from here to the end of the string
                        attributes[index] = color;
                        int j = index + 1;
                        while (j < text.Length)
                        {
                            if (text[j] == '\"')
                            {
                                attributes[j] = color;
                                ++j;
                                break;
                            }
                            else
                            {
                                attributes[j] = color;
                                ++j;
                            }
                        }
                        s = State.None;
                        index = j;
                    }
                    else if ((index == 0 || IsWhitespace(text[index - 1])) &&
                        index <= length - 3 &&
                        IsLetter(text[index + 0]) && IsLetter(text[index + 1]) && IsLetter(text[index + 2]) &&
                        IsWhitespaceOrDelimiterAt(index + 3))
                    {
                        // Check for opcode
                        char[] chars = new char[3];
                        chars[0] = text[index + 0];
                        chars[1] = text[index + 1];
                        chars[2] = text[index + 2];
                        string s = new string(chars).ToUpper();

                        bool foundOpcode = false;

                        for (int i = 0; i < allOpcodes.Length; ++i)
                        {
                            if (allOpcodes[i] == s)
                            {
                                attributes[index + 0] = (uint)Colors.Keyword_Blue;
                                attributes[index + 1] = (uint)Colors.Keyword_Blue;
                                attributes[index + 2] = (uint)Colors.Keyword_Blue;
                                foundOpcode = true;
                                break;
                            }
                        }

                        if (foundOpcode)
                        {
                            index += 3;
                        }
                        else
                        {
                            index++;
                        }

                    }
                    else
                    {
                        index++;
                    }

                }
            }

            bool IsWhitespaceOrDelimiterAt(int index)
            {
                return index >= text.Length || IsWhitespace(text[index]);
            }


            bool IsWhitespace(char c)
            {
                return c == ' ' || c == '\t' || c == '\r' || c == '\n';
            }
            bool IsCharacter(char c)
            {
                return !IsWhitespace(c);
            }

        }


        int IVsColorizer.ColorizeLine(int line, int length, IntPtr pszText, int state, uint[] attributes)
        {
            if (length == 0)
                return 0;

            Parser p = new Parser(length, pszText, attributes);

            p.EatLabel();

            p.ProcessLine();

            return 0;
        }

        int IVsColorizer.GetStateAtEndOfLine(int line, int length, IntPtr pszText, int state)
        {
            return 0;
        }
        void IVsColorizer.CloseColorizer()
        {
        }
    }
}
