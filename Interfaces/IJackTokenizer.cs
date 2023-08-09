using JackCompiler.Entities;

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
        /// Returns next token with value and type.
        /// </summary>
        /// <returns></returns>
        Token Advance();

    }
}
