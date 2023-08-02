namespace JackCompiler.Interfaces
{
    public interface ICompilationEngine
    {
        void CompileClass();

        void CompileClassVarDec();

        void CompileSubroutineDec();

        void CompileParameterList();

        void CompileSubroutineBody();

        void CompileVarDec();

        void CompileStatements();

        void CompileLet();

        void CompileIf();

        void CompileWhile();

        void CompileDo();

        void CompileReturn();

        void CompileExpression();

        void CompileTerm();

        int CompileExpressionList();
    }
}