using JackCompiler.Interfaces;
using System.Xml;

namespace JackCompiler.Modules
{
    public class CompilationEngine : ICompilationEngine
    {
        XmlWriter writer;
        JackTokenizer tokenizer;
        Token currentToken;

        public CompilationEngine(StreamReader sr, XmlWriter writer)
        {
            this.writer = writer;
            tokenizer = new JackTokenizer(sr);
            currentToken = tokenizer.Advance();
            while (currentToken.Type == "comment")
            {
                currentToken = tokenizer.Advance();
            }
        }

        public void CompileClass()
        {
            writer.WriteStartElement("class");
            ProcessKeywordOrSymbol("class");
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
            writer.WriteFullEndElement();
        }

        public void CompileClassVarDec()
        {
            writer.WriteStartElement("classVarDec");

            if (IsClassVarDec())
                ProcessKeywordOrSymbol(currentToken.Value);
            else
                throw new ArgumentException("Syntax error.");

            CompileType();
            ProcessConstantOrIdentifier("identifier");
            while (currentToken.Value == ",")
            {
                ProcessKeywordOrSymbol(",");
                ProcessConstantOrIdentifier("identifier");
            }
            ProcessKeywordOrSymbol(";");
            writer.WriteFullEndElement();
        }

        public void CompileDo()
        {
            writer.WriteStartElement("doStatement");
            ProcessKeywordOrSymbol("do");
            ProcessConstantOrIdentifier("identifier");
            if (currentToken.Value == "(")
            {
                ProcessKeywordOrSymbol("(");
                //expressionList start
                if (currentToken.Type == "identifier")
                {
                    ProcessConstantOrIdentifier("identifier");
                    while (currentToken.Value == ",")
                    {
                        ProcessKeywordOrSymbol(",");
                        ProcessConstantOrIdentifier("identifier");
                    }
                }
                //expressionList end
                ProcessKeywordOrSymbol(")");
            }
            else
            {
                ProcessKeywordOrSymbol(".");
                ProcessConstantOrIdentifier("identifier");
                ProcessKeywordOrSymbol("(");
                //expressionList start
                if (currentToken.Type == "identifier")
                {
                    ProcessConstantOrIdentifier("identifier");
                    while (currentToken.Value == ",")
                    {
                        ProcessKeywordOrSymbol(",");
                        ProcessConstantOrIdentifier("identifier");
                    }
                }
                if (currentToken.Type == "keyword")
                {
                    ProcessKeywordOrSymbol(currentToken.Value);
                }
                //expressionList end
                ProcessKeywordOrSymbol(")");
            }
            ProcessKeywordOrSymbol(";");
            writer.WriteFullEndElement();
        }

        public void CompileExpression()
        {
            throw new NotImplementedException();
        }

        public int CompileExpressionList()
        {
            throw new NotImplementedException();
        }

        public void CompileIf()
        {
            writer.WriteStartElement("ifStatement");
            ProcessKeywordOrSymbol("if");
            ProcessKeywordOrSymbol("(");
            //expression substitute start
            ProcessConstantOrIdentifier($"{currentToken.Type}");
            //expression substitute end
            ProcessKeywordOrSymbol(")");
            ProcessKeywordOrSymbol("{");
            CompileStatements();
            ProcessKeywordOrSymbol("}");
            if (currentToken.Value == "else")
            {
                ProcessKeywordOrSymbol("else");
                ProcessKeywordOrSymbol("{");
                CompileStatements();
                ProcessKeywordOrSymbol("}");
            }
            writer.WriteFullEndElement();
        }

        public void CompileLet()
        {
            writer.WriteStartElement("letStatement");
            ProcessKeywordOrSymbol("let");
            ProcessConstantOrIdentifier("identifier");
            if (currentToken.Value == "[")
            {
                ProcessKeywordOrSymbol("[");
                //expression
                ProcessKeywordOrSymbol("]");
            }
            ProcessKeywordOrSymbol("=");
            //expression substitute start
            ProcessConstantOrIdentifier($"{currentToken.Type}");
            //expression substitute end
            ProcessKeywordOrSymbol(";");
            writer.WriteFullEndElement();
        }

        public void CompileParameterList()
        {
            writer.WriteStartElement("parameterList");
            if (currentToken.Value == ")")
            {
                writer.WriteFullEndElement();
                return;
            }
            else
            {
                CompileType();
                ProcessConstantOrIdentifier("identifier");
                while (currentToken.Value == ",")
                {
                    ProcessKeywordOrSymbol(",");
                    CompileType();
                    ProcessConstantOrIdentifier("identifier");
                }
                writer.WriteFullEndElement();
            }
        }

        public void CompileReturn()
        {
            writer.WriteStartElement("returnStatement");
            ProcessKeywordOrSymbol("return");
            //expression start
            if (currentToken.Type == "identifier")
                ProcessConstantOrIdentifier("identifier");
            //expression end
            ProcessKeywordOrSymbol(";");
            writer.WriteFullEndElement();
        }

        public void CompileStatements()
        {
            string[] statements = { "let", "if", "while", "do", "return" };
            writer.WriteStartElement("statements");
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
            writer.WriteFullEndElement();
        }

        public void CompileSubroutineDec()
        {
            writer.WriteStartElement("subroutineDec");
            if (IsSubroutineDec())
                ProcessKeywordOrSymbol(currentToken.Value);
            else
                throw new ArgumentException("Syntax error.");

            if (currentToken.Value == "void")
                ProcessKeywordOrSymbol("void");
            else
                CompileType();

            ProcessConstantOrIdentifier("identifier");
            ProcessKeywordOrSymbol("(");
            CompileParameterList();
            ProcessKeywordOrSymbol(")");
            CompileSubroutineBody();
            writer.WriteFullEndElement();
        }

        public void CompileSubroutineBody()
        {
            writer.WriteStartElement("subroutineBody");
            ProcessKeywordOrSymbol("{");
            while (currentToken.Value == "var")
            {
                CompileVarDec();
            }
            CompileStatements();
            ProcessKeywordOrSymbol("}");

            writer.WriteFullEndElement();
        }

        public void CompileTerm()
        {
            throw new NotImplementedException();
        }

        public void CompileVarDec()
        {
            writer.WriteStartElement("varDec");
            ProcessKeywordOrSymbol("var");
            CompileType();
            ProcessConstantOrIdentifier("identifier");
            while (currentToken.Value == ",")
            {
                ProcessKeywordOrSymbol(",");
                ProcessConstantOrIdentifier("identifier");
            }
            ProcessKeywordOrSymbol(";");
            writer.WriteFullEndElement();
        }

        public void CompileWhile()
        {
            writer.WriteStartElement("whileStatement");
            ProcessKeywordOrSymbol("while");
            ProcessKeywordOrSymbol("(");
            //expression start
            if (currentToken.Type == "identifier")
                ProcessConstantOrIdentifier("identifier");
            //expression end
            ProcessKeywordOrSymbol(")");
            ProcessKeywordOrSymbol("{");
            CompileStatements();
            ProcessKeywordOrSymbol("}");
            writer.WriteFullEndElement();
        }

        private void ProcessKeywordOrSymbol(string expectedTokenValue)
        {
            if (currentToken.Type == null)
                return;

            if (currentToken.Value == expectedTokenValue)
            {
                writer.WriteStartElement($"{currentToken.Type}");
                writer.WriteString($" {expectedTokenValue} ");
                writer.WriteFullEndElement();
            }
            else
                throw new ArgumentException("Syntax error.");

            currentToken = tokenizer.Advance();

            while (currentToken.Type == "comment")
            {
                currentToken = tokenizer.Advance();
            }
        }

        private void ProcessConstantOrIdentifier(string expectedTokenType)
        {
            if (currentToken.Type == null)
                return;

            if (currentToken.Type == expectedTokenType)
            {
                writer.WriteStartElement($"{currentToken.Type}");
                writer.WriteString($" {currentToken.Value} ");
                writer.WriteFullEndElement();
            }
            else
                throw new ArgumentException("Syntax error.");

            currentToken = tokenizer.Advance();

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
    }
}
