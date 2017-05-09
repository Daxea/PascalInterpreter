using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalInterpreter
{
    public class SymanticAnalyzer : NodeVisitior
    {
        private ScopedSymbolTable _currentScope;

        public SymanticAnalyzer()
        {
            
        }

        private void Visit(BlockNode node)
        {
            foreach (var declaration in node.Declarations)
                Visit(declaration);
            Visit(node.Compound);
        }

        private void Visit(ProgramNode node)
        {
            Console.WriteLine("Enter Scope: global");
            var globalScope = new ScopedSymbolTable("global", 0);
            _currentScope = globalScope;

            Visit(node.Block);

            _currentScope = _currentScope.ParentScope;

            Console.WriteLine(globalScope);
            Console.WriteLine("Exit Scope: global");
        }

        private void Visit(ProcedureDeclarationNode node)
        {
            var procName = node.Name;
            var procSymbol = new ProcedureSymbol(procName);
            _currentScope.Define(procSymbol);

            Console.WriteLine($"Enter Scope: {procName}");
            var procedureScope = new ScopedSymbolTable(procName, _currentScope);
            _currentScope = procedureScope;

            foreach (var param in node.Parameters)
            {
                var paramType = _currentScope.Lookup<TypeSymbol>(param.Type.Value);
                var paramName = param.Variable.Value.As<string>();
                var variableSymbol = new VariableSymbol(paramName, paramType);
                _currentScope.Define(variableSymbol);
                procSymbol.Parameters.Add(variableSymbol);
            }

            Visit(node.Block);

            _currentScope = _currentScope.ParentScope;

            Console.WriteLine(procedureScope);
            Console.WriteLine($"Exit Scope: {procName}");
        }

        private void Visit(CompoundNode node)
        {
            foreach (var child in node.Children)
                Visit(child);
        }

        private void Visit(NoOpNode node) { /* Do Nothing */ }

        private void Visit(VariableDeclarationNode node)
        {
            var typeName = node.Type.Value;
            var typeSymbol = _currentScope.Lookup<TypeSymbol>(typeName);

            var name = node.Variable.Value.As<string>();
            var varSymbol = new VariableSymbol(name, typeSymbol);

            // Check for any symbol with the specified name
            // so we don't accidentally end up with types or methods
            // with the same name as a variable. :D
            if (_currentScope.Lookup<Symbol>(name, limitSearch: true) != null)
            {
                ReportError(new Exception($"{name} has already been defined! Please use unique identifiers for members."));
                return;
            }

            _currentScope.Define(varSymbol);
        }

        private void Visit(VariableNode node)
        {
            var name = node.Value.As<string>();
            var symbol = _currentScope.Lookup<VariableSymbol>(name);
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