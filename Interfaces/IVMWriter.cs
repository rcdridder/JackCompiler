namespace JackCompiler.Interfaces
{
    public interface IVMWriter
    {
        /// <summary>
        /// Writes a VM push command.
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="i"></param>
        void WritePush(string segment, int i);
        /// <summary>
        /// Writes a VM pop command.
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="i"></param>
        void WritePop(string segment, int i);
        /// <summary>
        /// Writes a VM arithmetic-logical command.
        /// </summary>
        /// <param name="command"></param>
        void WriteArithmetic(string command);
        /// <summary>
        /// Writes a VM label command.
        /// </summary>
        /// <param name="label"></param>
        void WriteLabel(string label);
        /// <summary>
        /// Writes a VM goto command.
        /// </summary>
        /// <param name="label"></param>
        void WriteGoto(string label);
        /// <summary>
        /// Writes a VM if-goto command.
        /// </summary>
        /// <param name="label"></param>
        void WriteIf(string label);
        /// <summary>
        /// Writes a VM call command.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nArgs"></param>
        void WriteCall(string name, int nArgs);
        /// <summary>
        /// Writes a VM function command.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nVars"></param>
        void WriteFunction(string name, int nVars);
        /// <summary>
        /// Writes a VM return command.
        /// </summary>
        void WriteReturn();

    }
}
