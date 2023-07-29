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
            string outputFileName = $@"{Path.GetDirectoryName(args[0])}\{jackFileName}T.xml";

            StreamReader sr = new(args[0]);
            XmlWriter writer = XmlWriter.Create(outputFileName, xmlSettings);

            writer.WriteStartDocument();
            writer.WriteStartElement("tokens");
            JackTokenizer tokenizer = new(sr);
            while (tokenizer.HasMoreTokens())
            {
                tokenizer.Advance();
                writer.WriteStartElement(tokenizer.TokenType());
                writer.WriteString(tokenizer.TokenValue());
            }
            writer.WriteEndDocument();
            writer.Close();
        }
        else if (Directory.Exists(args[0]))
        {
            string outputFileName = $@"{args[0]}\{Path.GetFileName(args[0])}T.xml";
            string[] vmFilesInDirectory = Directory.GetFiles($@"{args[0]}", "*.vm");
            foreach (string file in vmFilesInDirectory)
            {
                StreamReader sr = new(file);
                XmlWriter writer = XmlWriter.Create(outputFileName);
                JackTokenizer tokenizer = new(sr);
                //call tokenizer
            }
        }
        else
            throw new ArgumentException("Invalid file or directory.");
    }
}