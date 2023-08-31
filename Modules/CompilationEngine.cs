using JackCompiler.Entities;
using JackCompiler.Interfaces;

namespace JackCompiler.Modules
{
    public class CompilationEngine : ICompilationEngine
    {
        JackTokenizer tokenizer;
        VMWriter writer;
        Token currentToken;
        SymbolTable classTable = new SymbolTable();
        SymbolTable subroutineTable = new SymbolTable();
        string? currentClassName;
        string? currentSubroutineName;
        int labelCount = 0;
        bool isConstructor, isMethod = false;

        public CompilationEngine(StreamReader reader, StreamWriter writer)
        {
            tokenizer = new JackTokenizer(reader);
            this.writer = new VMWriter(writer);
            currentToken = tokenizer.Advance();
            while (currentToken.Type == "comment")
            {
                currentToken = tokenizer.Advance();
            }
        }

        public void CompileClass()
        {
            classTable.Reset();
            ProcessKeywordOrSymbol("class");
            currentClassName = currentToken.Value;
            ProcessConstantOrIdentifier("identifier");
            ProcessKeywordOrSymbol("{");
            while (IsClassVarDec())
            {
                CompileClassVarDec();
            }
            while (IsSubroutineDec())
            {
                CompileSubroutineDec();
            }
            ProcessKeywordOrSymbol("}");
        }

        public void CompileClassVarDec()
        {
            Symbol symbol = new();

            if (IsClassVarDec())
            {
                symbol.Kind = currentToken.Value;
                ProcessKeywordOrSymbol(currentToken.Value);
            }
            else
                throw new ArgumentException("Syntax error.");

            symbol.Type = currentToken.Value;
            CompileType();
            symbol.Name = currentToken.Value;
            classTable.Define(symbol.Name, symbol.Type, symbol.Kind);
            ProcessConstantOrIdentifier("identifier");
            while (currentToken.Value == ",")
            {
                ProcessKeywordOrSymbol(",");
                symbol.Name = currentToken.Value;
                classTable.Define(symbol.Name, symbol.Type, symbol.Kind);
                ProcessConstantOrIdentifier("identifier");
            }
            ProcessKeywordOrSymbol(";");
        }

        public void CompileDo()
        {
            ProcessKeywordOrSymbol("do");
            CompileExpression();
            writer.WritePop("temp", 0);
            ProcessKeywordOrSymbol(";");
        }

        public void CompileExpression()
        {
            string[] operands = { "+", "-", "*", "/", "&", "|", "<", ">", "=" };

            CompileTerm();
            while (operands.Contains(currentToken.Value))
            {
                string operand = currentToken.Value;
                ProcessKeywordOrSymbol(currentToken.Value);
                CompileTerm();
                switch (operand)
                {
                    case "+": writer.WriteArithmetic("add"); break;
                    case "-": writer.WriteArithmetic("sub"); break;
                    case "*": writer.WriteCall("Math.multiply", 2); break;
                    case "/": writer.WriteCall("Math.divide", 2); break;
                    case "&": writer.WriteArithmetic("and"); break;
                    case "|": writer.WriteArithmetic("or"); break;
                    case "<": writer.WriteArithmetic("lt"); break;
                    case ">": writer.WriteArithmetic("gt"); break;
                    case "=": writer.WriteArithmetic("eq"); break;
                }
            }
        }

        public int CompileExpressionList()
        {
            int expressionCount = 0;
            if (currentToken.Value == ")")
            {
                return expressionCount;
            }
            else
            {
                CompileExpression();
                expressionCount++;
                while (currentToken.Value == ",")
                {
                    ProcessKeywordOrSymbol(",");
                    CompileExpression();
                    expressionCount++;
                }
            }
            return expressionCount;
        }

        public void CompileIf()
        {
            string ifNotLabel = $"IfNot{currentClassName}{labelCount}";
            labelCount++;

            ProcessKeywordOrSymbol("if");
            ProcessKeywordOrSymbol("(");
            CompileExpression();
            ProcessKeywordOrSymbol(")");
            writer.WriteArithmetic("not");
            writer.WriteIf(ifNotLabel);
            ProcessKeywordOrSymbol("{");
            CompileStatements();
            ProcessKeywordOrSymbol("}");

            if (currentToken.Value == "else")
            {
                string endLabel = $"EndIf{currentClassName}{labelCount}";
                writer.WriteGoto(endLabel);
                writer.WriteLabel(ifNotLabel);
                ProcessKeywordOrSymbol("else");
                ProcessKeywordOrSymbol("{");
                CompileStatements();
                ProcessKeywordOrSymbol("}");
                writer.WriteLabel(endLabel);
            }
            else
                writer.WriteLabel(ifNotLabel);

        }

        public void CompileLet()
        {
            bool isArray = false;
            ProcessKeywordOrSymbol("let");
            string varName = currentToken.Value;
            ProcessConstantOrIdentifier("identifier", true);
            if (currentToken.Value == "[")
            {
                isArray = true;
                ProcessKeywordOrSymbol("[");
                writer.WritePush(FindSegment(varName), FindIndex(varName));
                CompileExpression();
                writer.WriteArithmetic("add");
                ProcessKeywordOrSymbol("]");
            }
            ProcessKeywordOrSymbol("=");
            CompileExpression();
            if (isArray)
            {
                writer.WritePop("temp", 0);
                writer.WritePop("pointer", 1);
                writer.WritePush("temp", 0);
                writer.WritePop("that", 0);
            }
            else
            {
                writer.WritePop(FindSegment(varName), FindIndex(varName));
            }
            ProcessKeywordOrSymbol(";");
        }

        public void CompileParameterList()
        {
            Symbol symbol = new();
            if (currentToken.Value == ")")
            {
                return;
            }
            else
            {
                symbol.Type = currentToken.Value;
                CompileType();
                symbol.Name = currentToken.Value;
                subroutineTable.Define(symbol.Name, symbol.Type, "arg");
                ProcessConstantOrIdentifier("identifier");
                while (currentToken.Value == ",")
                {
                    ProcessKeywordOrSymbol(",");
                    symbol.Type = currentToken.Value;
                    CompileType();
                    symbol.Name = currentToken.Value;
                    subroutineTable.Define(symbol.Name, symbol.Type, "arg");
                    ProcessConstantOrIdentifier("identifier");
                }
            }
        }

        public void CompileReturn()
        {
            ProcessKeywordOrSymbol("return");
            if (currentToken.Value == ";")
            {
                writer.WriteReturn();
                ProcessKeywordOrSymbol(";");
            }
            else
            {
                CompileExpression();
                writer.WriteReturn();
                ProcessKeywordOrSymbol(";");
            }
        }

        public void CompileStatements()
        {
            string[] statements = { "let", "if", "while", "do", "return" };
            while (statements.Contains(currentToken.Value))
            {
                switch (currentToken.Value)
                {
                    case "let": CompileLet(); break;
                    case "if": CompileIf(); break;
                    case "while": CompileWhile(); break;
                    case "do": CompileDo(); break;
                    case "return": CompileReturn(); break;
                }
            }
        }

        public void CompileSubroutineDec()
        {
            subroutineTable.Reset();

            if (IsSubroutineDec())
            {
                if (currentToken.Value == "method")
                {
                    isMethod = true;
                    subroutineTable.Define("this", currentClassName, "arg");
                    ProcessKeywordOrSymbol(currentToken.Value);
                }
                else if (currentToken.Value == "constructor")
                {
                    isConstructor = true;
                    ProcessKeywordOrSymbol(currentToken.Value);
                }
                else
                    ProcessKeywordOrSymbol(currentToken.Value);
            }
            else
                throw new ArgumentException("Syntax error.");

            if (currentToken.Value == "void")
                ProcessKeywordOrSymbol("void");
            else
                CompileType();

            currentSubroutineName = currentToken.Value;
            ProcessConstantOrIdentifier("identifier");
            ProcessKeywordOrSymbol("(");
            CompileParameterList();
            ProcessKeywordOrSymbol(")");
            CompileSubroutineBody();
        }

        public void CompileSubroutineBody()
        {
            ProcessKeywordOrSymbol("{");
            while (currentToken.Value == "var")
            {
                CompileVarDec();
            }
            writer.WriteFunction($"{currentClassName}.{currentSubroutineName}", subroutineTable.VarCount("var"));
            if (isConstructor)
            {
                writer.WritePush("constant", classTable.VarCount("field"));
                writer.WriteCall("Memory.alloc", 1);
                writer.WritePop("pointer", 0);
                isConstructor = false;
            }
            else if (isMethod)
            {
                writer.WritePush("argument", 0);
                writer.WritePop("pointer", 0);
                isMethod = false;
            }
            CompileStatements();
            ProcessKeywordOrSymbol("}");
        }

        public void CompileTerm()
        {
            string[] keywordConstants = { "true", "false", "null", "this" };

            if (currentToken.Type == "integerConstant")
            {
                writer.WritePush("constant", Convert.ToInt32(currentToken.Value));
                ProcessConstantOrIdentifier(currentToken.Type);
            }
            else if (currentToken.Type == "stringConstant")
            {
                writer.WritePush("constant", currentToken.Value.Length);
                writer.WriteCall("String.new", 1);
                foreach (char c in currentToken.Value)
                {
                    writer.WritePush("constant", c);
                    writer.WriteCall("String.appendChar", 2);
                }
                ProcessConstantOrIdentifier(currentToken.Type);
            }
            else if (keywordConstants.Contains(currentToken.Value))
            {
                switch (currentToken.Value)
                {
                    case "true":
                        writer.WritePush("constant", 1);
                        writer.WriteArithmetic("neg");
                        break;
                    case "false": case "null": writer.WritePush("constant", 0); break;
                    case "this": writer.WritePush("pointer", 0); break;
                }
                ProcessKeywordOrSymbol(currentToken.Value);
            }
            else if (currentToken.Value == "-")
            {
                ProcessKeywordOrSymbol(currentToken.Value);
                CompileTerm();
                writer.WriteArithmetic("neg");
            }
            else if (currentToken.Value == "~")
            {
                ProcessKeywordOrSymbol(currentToken.Value);
                CompileTerm();
                writer.WriteArithmetic("not");
            }
            else if (currentToken.Value == "(")
            {
                ProcessKeywordOrSymbol("(");
                CompileExpression();
                ProcessKeywordOrSymbol(")");
            }
            else if (currentToken.Type == "identifier")
            {
                string identifier = currentToken.Value;
                ProcessConstantOrIdentifier("identifier", true);
                if (currentToken.Value == "[")
                {
                    ProcessKeywordOrSymbol("[");
                    writer.WritePush(FindSegment(identifier), FindIndex(identifier));
                    CompileExpression();
                    writer.WriteArithmetic("add");
                    writer.WritePop("pointer", 1);
                    writer.WritePush("that", 0);
                    ProcessKeywordOrSymbol("]");
                }
                else if (currentToken.Value == "(")
                {
                    ProcessKeywordOrSymbol("(");
                    int argCount = CompileExpressionList();
                    ProcessKeywordOrSymbol(")");
                    writer.WritePush("pointer", 0);
                    writer.WriteCall($"{currentClassName}.{identifier}", argCount + 1);
                }
                else if (currentToken.Value == ".")
                {
                    ProcessKeywordOrSymbol(".");
                    string functionName = currentToken.Value;
                    ProcessConstantOrIdentifier("identifier", true);
                    ProcessKeywordOrSymbol("(");
                    int argCount = CompileExpressionList();
                    ProcessKeywordOrSymbol(")");
                    if (subroutineTable.SymbolExists(identifier) || classTable.SymbolExists(identifier))
                    {
                        writer.WritePush($"{FindSegment(identifier)}", FindIndex(identifier));
                        writer.WriteCall($"{FindType(identifier)}.{functionName}", argCount + 1);
                    }
                    else
                    {
                        writer.WriteCall($"{identifier}.{functionName}", argCount);
                    }
                }
                else
                    writer.WritePush(FindSegment(identifier), FindIndex(identifier));
            }
            else
            {
                throw new ArgumentException("Syntax error");
            }
        }

        public void CompileVarDec()
        {
            Symbol symbol = new();
            symbol.Kind = currentToken.Value;
            ProcessKeywordOrSymbol("var");
            symbol.Type = currentToken.Value;
            CompileType();
            symbol.Name = currentToken.Value;
            subroutineTable.Define(symbol.Name, symbol.Type, symbol.Kind);
            ProcessConstantOrIdentifier("identifier");
            while (currentToken.Value == ",")
            {
                ProcessKeywordOrSymbol(",");
                symbol.Name = currentToken.Value;
                subroutineTable.Define(symbol.Name, symbol.Type, symbol.Kind);
                ProcessConstantOrIdentifier("identifier");
            }
            ProcessKeywordOrSymbol(";");
        }

        public void CompileWhile()
        {
            string whileLabel = $"While{currentClassName}{labelCount}";
            string endLabel = $"EndWhile{currentClassName}{labelCount}";
            labelCount++;
            ProcessKeywordOrSymbol("while");
            writer.WriteLabel(whileLabel);
            ProcessKeywordOrSymbol("(");
            CompileExpression();
            ProcessKeywordOrSymbol(")");
            writer.WriteArithmetic("not");
            writer.WriteIf(endLabel);
            ProcessKeywordOrSymbol("{");
            CompileStatements();
            ProcessKeywordOrSymbol("}");
            writer.WriteGoto(whileLabel);
            writer.WriteLabel(endLabel);
        }

        private void ProcessKeywordOrSymbol(string expectedTokenValue)
        {
            if (currentToken.Type == null)
                return;

            if (currentToken.Value == expectedTokenValue)
                currentToken = tokenizer.Advance();
            else
                throw new ArgumentException("Syntax error.");

            while (currentToken.Type == "comment")
            {
                currentToken = tokenizer.Advance();
            }
        }

        private void ProcessConstantOrIdentifier(string expectedTokenType, bool isDeclared = false)
        {
            if (currentToken.Type == null)
                return;

            if (currentToken.Type == expectedTokenType)
                currentToken = tokenizer.Advance();
            else
                throw new ArgumentException("Syntax error.");

            while (currentToken.Type == "comment")
            {
                currentToken = tokenizer.Advance();
            }
        }

        private bool IsClassVarDec() => currentToken.Value == "static" || currentToken.Value == "field";

        private bool IsSubroutineDec()
        {
            return currentToken.Value == "constructor" ||
                currentToken.Value == "function" ||
                currentToken.Value == "method";
        }

        private void CompileType()
        {
            if (currentToken.Value == "int" || currentToken.Value == "char" || currentToken.Value == "boolean")
                ProcessKeywordOrSymbol(currentToken.Value);
            else if (currentToken.Type == "identifier")
                ProcessConstantOrIdentifier(currentToken.Type);
            else
                throw new ArgumentException("Syntax error.");
        }

        private string FindSegment(string identifier)
        {
            Dictionary<string, string> kindToSegment = new()
            {
                { "static", "static" },
                { "field", "this" },
                { "arg", "argument" },
                { "var", "local" },
            };
            if (subroutineTable.SymbolExists(identifier))
                return kindToSegment[subroutineTable.KindOf(identifier)];
            else if (classTable.SymbolExists(identifier))
                return kindToSegment[classTable.KindOf(identifier)];
            else
                throw new ArgumentException("Identifier not declared.");
        }

        private int FindIndex(string identifier)
        {
            if (subroutineTable.SymbolExists(identifier))
                return subroutineTable.IndexOf(identifier);
            else if (classTable.SymbolExists(identifier))
                return classTable.IndexOf(identifier);
            else
                throw new ArgumentException("Identifier not declared.");
        }

        private string FindType(string identifier)
        {
            if (subroutineTable.SymbolExists(identifier))
                return subroutineTable.TypeOf(identifier);
            else if (classTable.SymbolExists(identifier))
                return classTable.TypeOf(identifier);
            else
                return currentClassName;
        }
    }
}
