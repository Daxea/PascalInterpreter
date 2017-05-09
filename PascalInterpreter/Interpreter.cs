using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PascalInterpreter
{
    public class NodeVisitior
    {
        private readonly Dictionary<Type, MethodInfo> _methods;

        private List<Exception> _errors = new List<Exception>();
        public Exception[] Errors => _errors.ToArray();

        public bool IsSuccess => _errors.Count == 0;

        public NodeVisitior()
        {
            var type = GetType().GetTypeInfo();
            _methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => m.Name.StartsWith("Visit") && m.GetParameters().Length == 1)
                .ToDictionary(m => m.GetParameters().First().ParameterType);
        }

        public object Visit(AstNode node)
        {
            if (!_methods.ContainsKey(node.GetType()))
                return null;
            return _methods[node.GetType()].Invoke(this, new[] { node });
        }

        protected void ReportError(Exception error) => _errors.Add(error);
    }

    public class Interpreter : NodeVisitior
    {
        public Dictionary<string, object> GlobalVariables = new Dictionary<string, object>();

        private void Visit(CompoundNode node)
        {
            foreach (var child in node.Children)
                Visit(child);
        }

        private void Visit(NoOpNode node) { /* Do nothing! */ }

        private void Visit(AssignNode node)
        {
            var variableName = (node.Left as VariableNode)?.Value.As<string>() ?? string.Empty;
            if (string.IsNullOrEmpty(variableName))
                throw new Exception("cannot have a variable with no name!");
            GlobalVariables.Add(variableName, Visit(node.Right));
        }

        private object Visit(VariableNode node)
        {
            var name = node.Value.As<string>();
            if (GlobalVariables.ContainsKey(name))
                return GlobalVariables[name];
            throw new Exception($"no variables named {name}");
        }

        private object Visit(UnaryOpNode node)
        {
            if (node.Operation.Type == TokenTypes.Addition)
                return +Visit(node.Expression).As<int>();
            else if (node.Operation.Type == TokenTypes.Subtraction)
                return -Visit(node.Expression).As<int>();
            throw new Exception("That ain't no unary op");
        }

        private object Visit(BinOpNode node)
        {
            Func<object, object, object> add = (left, right) =>
            {
                if (left is float && right is float)
                    return (float)left + (float)right;
                if (left is float && right is int)
                    return (float)left + (int)right;
                if (left is int && right is float)
                    return (int)left + (float)right;
                if (left is int && right is int)
                    return (int)left + (int)right;
                return null;
            };

            Func<object, object, object> sub = (left, right) =>
            {
                if (left is float && right is float)
                    return (float)left - (float)right;
                if (left is float && right is int)
                    return (float)left - (int)right;
                if (left is int && right is float)
                    return (int)left - (float)right;
                if (left is int && right is int)
                    return (int)left - (int)right;
                return null;
            };

            Func<object, object, object> mult = (left, right) =>
            {
                if (left is float && right is float)
                    return (float)left * (float)right;
                if (left is float && right is int)
                    return (float)left * (int)right;
                if (left is int && right is float)
                    return (int)left * (float)right;
                if (left is int && right is int)
                    return (int)left * (int)right;
                return null;
            };

            Func<object, object, object> div = (left, right) =>
            {
                if (left is float && right is float)
                    return (float)left / (float)right;
                if (left is float && right is int)
                    return (float)left / (int)right;
                if (left is int && right is float)
                    return (int)left / (float)right;
                if (left is int && right is int)
                    return (int)left / (int)right;
                return null;
            };

            var leftValue = Visit(node.Left);
            var rightValue = Visit(node.Right);

            if (node.Operation.Type == TokenTypes.Addition)
                return add(leftValue, rightValue);
            else if (node.Operation.Type == TokenTypes.Subtraction)
                return sub(leftValue, rightValue);
            else if (node.Operation.Type == TokenTypes.Multiplication)
                return mult(leftValue, rightValue);
            else if (node.Operation.Type == TokenTypes.Division)
                return div(leftValue, rightValue);
            else if (node.Operation.Type == TokenTypes.RealDivision)
                return div(leftValue, rightValue);
            throw new Exception("That ain't no binop");
        }

        private object Visit(NumberNode node)
        {
            if (node.Value is int)
                return node.Value.As<int>();
            else if (node.Value is float)
                return node.Value.As<float>();
            throw new Exception("NaN");
        }

        private void Visit(ProgramNode node) => Visit(node.Block);

        private void Visit(BlockNode node)
        {
            foreach (var declaration in node.Declarations)
                Visit(declaration);
            Visit(node.Compound);
        }

        private void Visit(VariableDeclarationNode node)
        {

        }

        private void Visit(ProcedureDeclarationNode node) { }

        private void Visit(TypeNode node)
        {

        }

        public object Interpret(AstNode tree) => Visit(tree);
    }
}