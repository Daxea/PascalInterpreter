using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalInterpreter
{
    public class SymanticAnalyzer : NodeVisitior
    {
        private SymbolTable _symbols;

        public SymanticAnalyzer()
        {
            _symbols = new SymbolTable();
        }

        private void Visit(BlockNode node)
        {
            foreach (var declaration in node.Declarations)
                Visit(declaration);
            Visit(node.Compound);
        }

        private void Visit(ProgramNode node) => Visit(node.Block);

        private void Visit(CompoundNode node)
        {
            foreach (var child in node.Children)
                Visit(child);
        }

        private void Visit(NoOpNode node) { /* Do Nothing */ }

        private void Visit(VariableDeclarationNode node)
        {
            var typeName = node.Type.Value;
            var typeSymbol = _symbols.Lookup<TypeSymbol>(typeName);

            var name = node.Variable.Value.As<string>();
            var varSymbol = new VariableSymbol(name, typeSymbol);

            // Check for any symbol with the specified name
            // so we don't accidentally end up with types or methods
            // with the same name as a variable. :D
            if (_symbols.Lookup<Symbol>(name) != null)
            {
                ReportError(new Exception($"{name} has already been defined! Please use unique identifiers for members."));
                return;
            }

            _symbols.Define(varSymbol);
        }

        private void Visit(VariableNode node)
        {
            var name = node.Value.As<string>();
            var symbol = _symbols.Lookup<VariableSymbol>(name);
            if (symbol == null)
                ReportError(new Exception($"Variable {name} has not been declared!"));
        }

        private void Visit(AssignNode node)
        {
            Visit(node.Left);
            Visit(node.Right);
        }

        private void Visit(BinOpNode node)
        {
            Visit(node.Left);
            Visit(node.Right);
        }
    }
}