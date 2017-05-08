using System;
using System.Collections.Generic;
using System.Linq;

namespace PascalInterpreter
{
    public class SymbolTableBuilder : NodeVisitior
    {
        private SymbolTable _table;

        public SymbolTableBuilder()
        {
            _table = new SymbolTable();
        }

        private void Visit(BlockNode node)
        {
            foreach (var declaration in node.Declarations)
                Visit(declaration);
            Visit(node.Compound);
        }

        private void Visit(ProgramNode node) => Visit(node.Block);

        private void Visit(BinOpNode node)
        {
            Visit(node.Left);
            Visit(node.Right);
        }

        private void Visit(NumberNode node) { }

        private void Visit(UnaryOpNode node) => Visit(node.Expression);

        private void Visit(CompoundNode node)
        {
            foreach (var child in node.Children)
                Visit(child);
        }

        private void Visit(NoOpNode node) { }

        private void Visit(VariableDeclarationNode node)
        {
            var typeName = node.Type.Value.As<string>();
            var typeSymbol = _table.Lookup<TypeSymbol>(typeName);
            var variableName = node.Variable.Value.As<string>();
            var variableSymbol = new VariableSymbol(variableName, typeSymbol);
            _table.Define(variableSymbol);
        }

        private void Visit(AssignNode node)
        {
            var name = node.Left.As<VariableNode>().Value.As<string>();
            var symbol = _table.Lookup<VariableSymbol>(name);
            if (symbol == null)
                throw new Exception("Define yo vars, pls.");
            Visit(node.Right);
        }

        private void Visit(VariableNode node)
        {
            var name = node.Value.As<string>();
            var symbol = _table.Lookup<VariableSymbol>(name);
            if (symbol == null)
                throw new Exception("Define yo vars, pls.");
        }

        private void Visit(ProcedureDeclarationNode node) { }

        public override string ToString() => _table.ToString();
    }
}