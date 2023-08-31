namespace JackCompiler.Interfaces
{
    public interface ISymbolTable
    {
        /// <summary>
        /// Empties the symbol table and resets the four indexes to 0;
        /// </summary>
        void Reset();
        /// <summary>
        /// Adds a new variable of the given name, type and kind. Assigns index value of that kind and adds 1 to that index.
        /// </summary>
        void Define(string name, string type, string kind);
        /// <summary>
        /// Returns the number of variables of the given kind defined in the symbol table.
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        int VarCount(string kind);
        /// <summary>
        /// Returns the kind of the named identifier. If the identifier isn't found, returns NONE.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string KindOf(string name);
        /// <summary>
        /// Returns the type of the named identifier.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string TypeOf(string name);
        /// <summary>
        /// Returns the index of the named identifier.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int IndexOf(string name);
        /// <summary>
        /// Determines if a symbol exists in the symbol table.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool SymbolExists(string name);
    }
}
