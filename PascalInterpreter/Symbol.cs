using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalInterpreter
{
    public class Symbol
    {
        public string Name { get; }

        public Symbol(string name)
        {
            Name = name;
        }

        public override string ToString() => $"Symbol: {Name}";
    }

    public class TypeSymbol : Symbol
    {
        public TypeSymbol(string name)
            : base(name)
        { }

        public override string ToString() => $"Type: {Name}";
    }

    public class VariableSymbol : Symbol
    {
        public TypeSymbol Type { get; }

        public VariableSymbol(string name, TypeSymbol type)
            : base(name)
        {
            Type = type;
        }

        public override string ToString() => $"Variable: {Name} : {Type}";
    }

    public class ProcedureSymbol : Symbol
    {
        public List<VariableSymbol> Parameters { get; }

        public ProcedureSymbol(string name, params VariableSymbol[] parameters)
            : base(name)
        {
            Parameters = new List<VariableSymbol>(parameters);
        }

        public override string ToString()
        {
            var paramDisplay = string.Empty;
            foreach (var param in Parameters)
                paramDisplay += $"\t{param.Name} : {param.Type.Name}\n";
            return $"Symbol: {Name}\n{paramDisplay}";
        }
    }
}