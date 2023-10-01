using JackCompiler.Modules;

public class Program
{
    private static void Main(string[] args)
    {

        if (Path.GetExtension(args[0]) == ".jack")
        {
            string jackFileName = Path.GetFileNameWithoutExtension(args[0]);
            string outputFileName = $"{Path.GetDirectoryName(args[0])}/{jackFileName}.vm";

            StreamReader sr = new(args[0]);
            StreamWriter writer = new(outputFileName);
            CompilationEngine engine = new(sr, writer);

            engine.CompileClass();
            writer.Close();
            Console.WriteLine($"{jackFileName}.jack compiled succesfully!");
        }
        else if (Directory.Exists(args[0]))
        {
            string[] vmFilesInDirectory = Directory.GetFiles($@"{args[0]}", "*.jack");

            foreach (string file in vmFilesInDirectory)
            {
                string jackFileName = Path.GetFileNameWithoutExtension(file);
                string outputFileName = $"{args[0]}/{jackFileName}.vm";

                StreamReader sr = new(file);
                StreamWriter writer = new(outputFileName);
                CompilationEngine engine = new(sr, writer);

                engine.CompileClass();
                writer.Close();
                Console.WriteLine($"{jackFileName}.jack compiled succesfully!");
            }
            Console.WriteLine($"All jack files in {args[0]} are compiled!");
        }
        else
            throw new ArgumentException("Invalid file or directory.");
    }
}