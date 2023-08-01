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

        public bool HasMoreTokens() => sr.Peek() >= 0;

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
                    token.Type = "comment";
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
                        token.Type = "comment";
                        return token;
                    }
                    else
                        goto StillComment;
                }
                else //slash symbol
                {
                    token.Type = "symbol";
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
                token.Type = "symbol";
                switch (currentChar)
                {
                    case '<': token.Value = "&lt"; break;
                    case '>': token.Value = "&gt"; break;
                    case '&': token.Value = "&amp"; break;
                    default: token.Value += currentChar; break;
                }
                return token;
            }
            else
                return token;
        }

        private Token IsIntegerConstant(Token token)
        {
            if (char.IsDigit(currentChar))
            {
                token.Value += currentChar;
                while (char.IsDigit((char)sr.Peek()))
                {
                    currentChar = (char)sr.Read();
                    token.Value += currentChar;
                }
                if (char.IsWhiteSpace((char)sr.Peek()) || symbols.Contains((char)sr.Peek()))
                {
                    token.Type = "integerConstant";
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
                while ((char)sr.Peek() != '"' && (char)sr.Peek() != '\n')
                {
                    currentChar = (char)sr.Read();
                    token.Value += currentChar;
                }
                currentChar = (char)sr.Read();
                token.Type = "stringConstant";
                return token;
            }
            else
                return token;
        }

        private Token IsIdentifier(Token token)
        {
            if (char.IsLetter(currentChar) || currentChar == '_')
            {
                while (char.IsDigit((char)sr.Peek()) || char.IsLetter((char)sr.Peek()) || (char)sr.Peek() == '_')
                {
                    token.Value += currentChar;
                    currentChar = (char)sr.Read();
                }
                token.Value += currentChar;

                if (char.IsWhiteSpace((char)sr.Peek()) || symbols.Contains((char)sr.Peek()))
                {
                    if (keywords.Contains(token.Value))
                        token.Type = "keyword";
                    else
                        token.Type = "identifier";
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
