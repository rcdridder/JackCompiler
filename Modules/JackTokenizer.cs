using JackCompiler.Entities;
using JackCompiler.Interfaces;

namespace JackCompiler.Modules
{
    public class JackTokenizer : IJackTokenizer
    {
        private StreamReader reader;
        private char[] symbols = { '{', '}', '(', ')', '[', ']', '.', ',', ';',
                '+', '-', '*', '/', '&', '|', '<', '>', '=', '~' };
        private string[] tokenTypes = { "keyword", "symbol", "identifier", "integerConstant", "stringConstant " };
        private string[] keywords = { "class", "method", "function", "constructor", "int", "boolean", "char", "void",
            "var", "static", "field", "let", "do" , "if", "else", "while", "return", "true", "false", "null", "this" };
        private char currentChar;
        private string currentToken = string.Empty;

        public JackTokenizer(StreamReader sr)
        {
            this.reader = sr;
        }

        public bool HasMoreTokens() => reader.Peek() >= 0;

        public Token Advance()
        {
            Token token = new Token();
            currentChar = (char)reader.Read();

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
                currentChar = (char)reader.Read();
            }

            if (currentChar == '/')
            {
                if ((char)reader.Peek() == '/') //one line comment
                {
                    reader.ReadLine();
                    token.Type = "comment";
                    return token;
                }
                else if ((char)reader.Peek() == '*') //multiline comment
                {
                    currentChar = (char)reader.Read();

                StillComment:
                    while (currentChar != '*')
                    {
                        currentChar = (char)reader.Read();
                    }

                    currentChar = (char)reader.Read();

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
                token.Value += currentChar;
                while (char.IsDigit((char)reader.Peek()))
                {
                    currentChar = (char)reader.Read();
                    token.Value += currentChar;
                }
                if (char.IsWhiteSpace((char)reader.Peek()) || symbols.Contains((char)reader.Peek()))
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
                while ((char)reader.Peek() != '"' && (char)reader.Peek() != '\n')
                {
                    currentChar = (char)reader.Read();
                    token.Value += currentChar;
                }
                currentChar = (char)reader.Read();
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
                while (char.IsDigit((char)reader.Peek()) || char.IsLetter((char)reader.Peek()) || (char)reader.Peek() == '_')
                {
                    token.Value += currentChar;
                    currentChar = (char)reader.Read();
                }
                token.Value += currentChar;

                if (char.IsWhiteSpace((char)reader.Peek()) || symbols.Contains((char)reader.Peek()))
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
