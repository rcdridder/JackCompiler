using JackCompiler.Interfaces;

namespace JackCompiler.Modules
{
    public class VMWriter : IVMWriter
    {
        StreamWriter writer;

        public VMWriter(StreamWriter writer)
        {
            this.writer = writer;
        }

        public void WriteArithmetic(string command) => writer.WriteLine(command);

        public void WriteCall(string name, int nArgs) => writer.WriteLine($"call {name} {nArgs}");

        public void WriteFunction(string name, int nVars) => writer.WriteLine($"function {name} {nVars}");

        public void WriteGoto(string label) => writer.WriteLine($"goto {label}");

        public void WriteIf(string label) => writer.WriteLine($"if-goto {label}");

        public void WriteLabel(string label) => writer.WriteLine($"label {label}");

        public void WritePop(string segment, int index) => writer.WriteLine($"pop {segment} {index}");

        public void WritePush(string segment, int index) => writer.WriteLine($"push {segment} {index}");

        public void WriteReturn() => writer.WriteLine("return");
    }
}
