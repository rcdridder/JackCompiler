using JackCompiler.Modules;
using System.Xml;

public class Program
{
    private static void Main(string[] args)
    {
        XmlWriterSettings xmlSettings = new();
        xmlSettings.Indent = true;

        if (Path.GetExtension(args[0]) == ".jack")
        {
            string jackFileName = Path.GetFileNameWithoutExtension(args[0]);
            string outputFileName = $"{Path.GetDirectoryName(args[0])}/{jackFileName}.xml";

            StreamReader sr = new(args[0]);
            XmlWriter writer = XmlWriter.Create(outputFileName, xmlSettings);
            CompilationEngine engine = new(sr, writer);

            writer.WriteStartDocument();
            engine.CompileClass();
            writer.WriteEndDocument();
            writer.Close();
        }
        else if (Directory.Exists(args[0]))
        {

            string[] vmFilesInDirectory = Directory.GetFiles($@"{args[0]}", "*.jack");

            foreach (string file in vmFilesInDirectory)
            {
                string jackFileName = Path.GetFileNameWithoutExtension(file);
                string outputFileName = $"{args[0]}/{jackFileName}.xml";

                StreamReader sr = new(file);
                XmlWriter writer = XmlWriter.Create(outputFileName, xmlSettings);
                CompilationEngine engine = new(sr, writer);

                writer.WriteStartDocument();
                engine.CompileClass();
                writer.WriteEndDocument();
                writer.Close();
            }
        }
        else
            throw new ArgumentException("Invalid file or directory.");
    }
}