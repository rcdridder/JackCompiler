namespace JackCompiler.Interfaces
{
    public interface IJackTokenizer
    {
        /// <summary>
        /// Indicates if there are more tokens in the input file.
        /// </summary>
        /// <returns></returns>
        bool HasMoreTokens();
        /// <summary>
        /// Groups characters from the input file into tokens and determines the type of token. 
        /// </summary>
        void Advance();
        /// <summary>
        /// Returns the type of the current token.
        /// </summary>
        /// <returns></returns>
        string TokenType();
        /// <summary>
        /// Returns the value of the current token.
        /// </summary>
        /// <returns></returns>
        string TokenValue();
    }
}
