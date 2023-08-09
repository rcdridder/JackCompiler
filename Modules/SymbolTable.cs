using JackCompiler.Entities;
using JackCompiler.Interfaces;

namespace JackCompiler.Modules
{
    public class SymbolTable : ISymbolTable
    {
        List<Symbol> symbolTable = new List<Symbol>();
        int staticCount = 0;
        int fieldCount = 0;
        int argCount = 0;
        int varCount = 0;

        public void Reset()
        {
            symbolTable.Clear();
            staticCount = 0;
            fieldCount = 0;
            argCount = 0;
            varCount = 0;
        }

        public void Define(string name, string type, string kind)
        {
            if (SymbolExists(name))
                throw new ArgumentException("Symbol already exists in this scope.");

            switch (kind)
            {
                case "static":
                    symbolTable.Add(new Symbol { Name = name, Type = type, Kind = kind, Index = staticCount });
                    staticCount++; break;
                case "field":
                    symbolTable.Add(new Symbol { Name = name, Type = type, Kind = kind, Index = fieldCount });
                    fieldCount++; break;
                case "arg":
                    symbolTable.Add(new Symbol { Name = name, Type = type, Kind = kind, Index = argCount });
                    argCount++; break;
                case "var":
                    symbolTable.Add(new Symbol { Name = name, Type = type, Kind = kind, Index = varCount });
                    varCount++; break;
            }
        }

        public int VarCount(string kind)
        {
            return symbolTable.Where(s => s.Kind == kind).Count();
        }

        public string KindOf(string name)
        {
            Symbol searchResult = symbolTable.Find(s => s.Name == name);

            if (searchResult == null)
            {
                return "NONE";
            }
            else
            {
                return searchResult.Kind;
            }
        }

        public string TypeOf(string name)
        {
            return symbolTable.Find(s => s.Name == name).Type;
        }

        public int IndexOf(string name)
        {
            return symbolTable.Find(s => s.Name == name).Index;
        }

        public bool SymbolExists(string name) => symbolTable.Exists(s => s.Name == name);
    }
}
