using JackCompiler.Interfaces;

namespace JackCompiler.Modules
{
    public class JackTokenizer : IJackTokenizer
    {
        private StreamReader sr;
        private char[] symbols = { '{', '}', '(', ')', '[', ']', '.', ',', ';',
                '+', '-', '*', '/', '&', '|', '<', '>', '=', '~' };
        private string[] tokenTypes = { "keyword", "symbol", "identifier", "integerConstant", "stringConstant " };
        private string[] keywords = { "class", "method", "function", "constructor", "int", "boolean", "char", "void",
            "var", "static", "field", "let", "do" , "if", "else", "while", "return", "true", "false", "null", "this" };
        private char currentChar;
        private string currentToken = string.Empty;

        public JackTokenizer(StreamReader sr)
        {
            this.sr = sr;
        }

        public bool HasMoreTokens() => sr.BaseStream.Position > -1;

        public Token Advance()
        {
            Token token = new Token();
            currentChar = (char)sr.Read();

            if (IsCommentOrWhiteSpace(token).Type != null)
                return token;
            if (IsSymbol(token).Type != null)
                return token;
            if (IsIntegerConstant(token).Type != null)
                return token;
            if (IsStringConstant(token).Type != null)
                return token;
            if (IsIdentifier(token).Type != null)
                return token;
            return token;
        }

        private Token IsCommentOrWhiteSpace(Token token)
        {
            while (char.IsWhiteSpace(currentChar))
            {
                currentChar = (char)sr.Read();
            }

            if (currentChar == '/')
            {
                if ((char)sr.Peek() == '/') //one line comment
                {
                    sr.ReadLine();
                    token.Type = "Comment";
                    return token;
                }
                else if ((char)sr.Peek() == '*') //multiline comment
                {
                    currentChar = (char)sr.Read();

                StillComment:
                    while (currentChar != '*')
                    {
                        currentChar = (char)sr.Read();
                    }

                    currentChar = (char)sr.Read();

                    if (currentChar == '/')
                    {
                        token.Type = "Comment";
                        return token;
                    }
                    else
                        goto StillComment;
                }
                else //slash symbol
                {
                    token.Type = "Symbol";
                    token.Value += currentChar;
                    return token;
                }
            }
            else
            {
                return token;
            }
        }

        private Token IsSymbol(Token token)
        {
            token.Value = string.Empty;
            if (symbols.Contains(currentChar))
            {
                token.Type = "Symbol";
                token.Value += currentChar;
                return token;
            }
            else
                return token;
        }

        private Token IsIntegerConstant(Token token)
        {
            if (char.IsDigit(currentChar))
            {
                while (char.IsDigit(currentChar))
                {
                    token.Value += currentChar;
                    currentChar = (char)sr.Read();
                }
                if (char.IsWhiteSpace(currentChar) || symbols.Contains(currentChar))
                {
                    sr.BaseStream.Position = sr.BaseStream.Position - 2;
                    currentChar = (char)sr.Read();
                    token.Type = "IntegerConstant";
                    return token;
                }
                else
                    throw new Exception("Integer can only be followed by whitespace or symbol.");
            }
            else
                return token;
        }

        private Token IsStringConstant(Token token)
        {
            if (currentChar == '"')
            {
                currentChar = (char)sr.Read();
                while (currentChar != '"' && currentChar != '\n')
                {
                    token.Value += currentChar;
                    currentChar = (char)sr.Read();
                }

                if (currentChar == '\n')
                    throw new Exception("No multiline strings allowed.");


                token.Type = "StringConstant";
                return token;

            }
            else
                return token;
        }

        private Token IsIdentifier(Token token)
        {
            if (char.IsLetter(currentChar) || currentChar == '_')
            {
                token.Value += currentChar;
                currentChar = (char)sr.Read();
                while (char.IsDigit(currentChar) || char.IsLetter(currentChar) || currentChar == '_')
                {
                    token.Value += currentChar;
                    currentChar = (char)sr.Read();
                }

                if (char.IsWhiteSpace(currentChar) || symbols.Contains(currentChar))
                {
                    sr.BaseStream.Position = sr.BaseStream.Position - 2;
                    currentChar = (char)sr.Read();
                    if (keywords.Contains(token.Value))
                        token.Type = "Keyword";
                    else
                        token.Type = "Identifier";
                    return token;
                }
                else
                    throw new Exception("Invalid must be followed by whitespace or symbol.");
            }
            else
                return token;

        }
    }
}
