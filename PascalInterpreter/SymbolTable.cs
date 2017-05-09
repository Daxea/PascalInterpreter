using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace PascalInterpreter
{
    public class SymbolTable
    {
        protected OrderedDictionary _symbols;

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

        public virtual T Lookup<T>(string name)
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

    public class ScopedSymbolTable : SymbolTable
    {
        public string ScopeName { get; }
        public int ScopeLevel { get; }
        public ScopedSymbolTable ParentScope { get; }

        public ScopedSymbolTable(string scopeName, int scopeLevel)
        {
            ScopeName = scopeName;
            ScopeLevel = scopeLevel;
        }

        public ScopedSymbolTable(string scopeName, ScopedSymbolTable parentScope) : base()
        {
            ScopeName = scopeName;
            ParentScope = parentScope;
            ScopeLevel = ParentScope.ScopeLevel + 1;
        }

        public override T Lookup<T>(string name)
        {
            return Lookup<T>(name, false);
        }

        public T Lookup<T>(string name, bool limitSearch)
            where T : Symbol
        {
            if (_symbols.Contains(name))
                return _symbols[name].As<T>();
            if (limitSearch)
                return null;
            if (ParentScope != null)
                return ParentScope.Lookup<T>(name);
            return null;
        }

        public override string ToString() => $"Scope: {ScopeLevel}-{ScopeName}\n{base.ToString()}";
    }
}