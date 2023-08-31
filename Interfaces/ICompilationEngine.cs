namespace JackCompiler.Interfaces
{
    public interface ICompilationEngine
    {
        /// <summary>
        /// Compiles a complete class.
        /// </summary>
        void CompileClass();
        /// <summary>
        /// Compiles a static variable declaration or a field declaration.
        /// </summary>
        void CompileClassVarDec();
        /// <summary>
        /// Compiles a complete method function or constructor. Also (optionally) adds statics and fields to class symbol table.
        /// </summary>
        void CompileSubroutineDec();
        /// <summary>
        /// Compiles a (possibly empty) parameter list. Excludes the enclosing parenthesis. If subroutine is a mehtod, also adds this to subroutine symbol table.
        /// </summary>
        void CompileParameterList();
        /// <summary>
        /// Compiles a subroutine body. Also (optionally) adds args to subroutine symbol table.
        /// </summary>
        void CompileSubroutineBody();
        /// <summary>
        /// Compiles a var declaration.
        /// </summary>
        void CompileVarDec();
        /// <summary>
        /// Compiles a sequence of statements. Excludes the enclosing curly brackets. Also adds vars to subroutine symbol table.
        /// </summary>
        void CompileStatements();
        /// <summary>
        /// Compiles a let statement.
        /// </summary>
        void CompileLet();
        /// <summary>
        /// Compiles an if statement (with else clause if applicable).
        /// </summary>
        void CompileIf();
        /// <summary>
        /// Compiles a while statement.
        /// </summary>
        void CompileWhile();
        /// <summary>
        /// Compiles a do statement.
        /// </summary>
        void CompileDo();
        /// <summary>
        /// Compiles a return statement.
        /// </summary>
        void CompileReturn();
        /// <summary>
        /// Compiles an expression.
        /// </summary>
        void CompileExpression();
        /// <summary>
        /// Compiles a term. If the current token is an identifier, it's resolved depending on the next token. A variable, arrayElement ('[') or subroutineCall ('.' or '(').
        /// </summary>
        void CompileTerm();
        /// <summary>
        /// Compiles a (possibly empty) comma-separated list of expression and returns the number of expressions in the list
        /// </summary>
        /// <returns></returns>
        int CompileExpressionList();
    }
}