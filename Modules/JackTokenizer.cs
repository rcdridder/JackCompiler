using JackCompiler.Interfaces;

namespace JackCompiler.Modules
{
    public class JackTokenizer : IJackTokenizer
    {
        private StreamReader sr;
        private string[] tokenTypes = { "keyword", "symbol", "identifier", "integerConstant", "stringConstant " };
        private string[] keywords = { "class", "method", "function", "constructor", "int", "boolean", "char", "void",
            "var", "static", "field", "let", "do" , "if", "else", "while", "return", "true", "false", "null", "this" };
        private string currentToken = string.Empty;

        public JackTokenizer(StreamReader sr)
        {
            this.sr = sr;
        }

        public bool HasMoreTokens()
        {
            throw new NotImplementedException();
        }

        public void Advance()
        {

        }

        public string TokenType()
        {
            throw new NotImplementedException();
        }

        public string TokenValue()
        {
            throw new NotImplementedException();
        }

    }
}
