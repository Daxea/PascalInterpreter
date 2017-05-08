using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace PascalInterpreter
{
    public class SymbolTable
    {
        private OrderedDictionary _symbols;

        public SymbolTable()
        {
            _symbols = new OrderedDictionary();
            BuildLanguageTypeSymbols();
        }

        private void BuildLanguageTypeSymbols()
        {
            Define(new TypeSymbol("INTEGER"));
            Define(new TypeSymbol("REAL"));
        }

        public void Define(Symbol symbol)
        {
            Console.WriteLine($"Define: {symbol}");
            _symbols.Add(symbol.Name, symbol);
        }

        public T Lookup<T>(string name)
            where T : Symbol
        {
            Console.WriteLine($"Lookup: {name}");
            if (_symbols.Contains(name))
                return _symbols[name].As<T>();
            return null;
        }

        public override string ToString()
        {
            var result = "Symbols:";
            for (int i = 0; i < _symbols.Count; i++)
                result += $"\n\t{_symbols[i]}";
            return result;
        }
    }
}