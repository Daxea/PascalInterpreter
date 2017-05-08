using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalInterpreter
{
    public abstract class AstNode { }

    public class ProgramNode : AstNode
    {
        public string Name { get; }
        public BlockNode Block { get; }

        public ProgramNode(string name, BlockNode block)
        {
            Name = name;
            Block = block;
        }
    }

    public class BlockNode : AstNode
    {
        public DeclarationNode[] Declarations { get; }
        public CompoundNode Compound { get; }

        public BlockNode(DeclarationNode[] declarations, CompoundNode compound)
        {
            Declarations = declarations;
            Compound = compound;
        }
    }

    public abstract class DeclarationNode : AstNode { }

    public class VariableDeclarationNode : DeclarationNode
    {
        public VariableNode Variable { get; }
        public TypeNode Type { get; }

        public VariableDeclarationNode(VariableNode variable, TypeNode type)
        {
            Variable = variable;
            Type = type;
        }
    }

    public class TypeNode : AstNode
    {
        public Token Token { get; }
        public string Value { get; }

        public TypeNode(Token token)
        {
            Token = token;
            Value = token.Value.As<string>();
        }
    }

    public class BinOpNode : AstNode
    {
        public AstNode Left { get; }
        public Token Operation { get; }
        public Token Token { get; }
        public AstNode Right { get; }
        public BinOpNode(AstNode left, Token operation, AstNode right)
        {
            Left = left;
            Operation = Token = operation;
            Right = right;
        }
    }

    public class UnaryOpNode : AstNode
    {
        public AstNode Expression { get; }
        public Token Operation { get; }
        public Token Token { get; }

        public UnaryOpNode(Token operation, AstNode expression)
        {
            Expression = expression;
            Operation = operation;
        }
    }

    public class NumberNode : AstNode
    {
        public Token Token { get; }
        public object Value { get; }

        public NumberNode(Token token)
        {
            Token = token;
            Value = token.Value;
        }
    }

    public class CompoundNode : AstNode
    {
        public AstNode[] Children { get; }

        public CompoundNode(params AstNode[] children)
        {
            Children = children;
        }
    }

    public class AssignNode : BinOpNode
    {
        public AssignNode(AstNode left, AstNode right) :
            base(left, new Token(TokenTypes.Assign, ":="), right)
        { }
    }

    public class VariableNode : AstNode
    {
        public Token Token { get; }
        public object Value { get; }

        public VariableNode(Token token)
        {
            Token = token;
            Value = token.Value;
        }
    }

    public class ProcedureDeclarationNode : DeclarationNode
    {
        public string Name { get; }
        public BlockNode Block { get; }

        public ProcedureDeclarationNode(string name, BlockNode block)
        {
            Name = name;
            Block = block;
        }
    }

    public class NoOpNode : AstNode { }

    public class Parser
    {
        private Lexer _lexer;
        private Token _currentToken;

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            _currentToken = _lexer.GetNextToken();
        }

        private void Eat(string tokenType)
        {
            if (_currentToken.Type == tokenType)
                _currentToken = _lexer.GetNextToken();
            else
                throw new Exception("Not edible...");
        }

        private AstNode Factor()
        {
            var token = _currentToken;
            if (TokenTypes.IsUnaryOp(token))
            {
                Eat(token.Type);
                return new UnaryOpNode(token, Factor());
            }
            if (new[] { TokenTypes.LiteralInteger, TokenTypes.LiteralReal }.Contains(token.Type))
            {
                Eat(token.Type);
                return new NumberNode(token);
            }
            else if (token.Type == TokenTypes.LeftParenthesis)
            {
                Eat(TokenTypes.LeftParenthesis);
                var node = Expression();
                Eat(TokenTypes.RightParenthesis);
                return node;
            }
            else
                return Variable();
            throw new Exception("No factoring how this happened into the equation...");
        }

        private AstNode Term()
        {
            var node = Factor();

            var ops = new[] { TokenTypes.RealDivision, TokenTypes.Multiplication, TokenTypes.Division };

            while (ops.Contains(_currentToken.Type))
            {
                var token = _currentToken;
                Eat(token.Type);
                node = new BinOpNode(node, token, Factor());
            }
            return node;
        }

        private AstNode Expression()
        {
            var node = Term();

            var ops = new[] { TokenTypes.Addition, TokenTypes.Subtraction };

            while (ops.Contains(_currentToken.Type))
            {
                var token = _currentToken;
                Eat(token.Type);
                node = new BinOpNode(node, token, Term());
            }

            return node;
        }

        public AstNode Parse() => Program();

        private AstNode Program()
        {
            Eat(TokenTypes.Program);
            var variableNode = Variable();
            var programName = variableNode.Value.As<string>();
            Eat(TokenTypes.Semi);
            var blockNode = Block();
            var programNode = new ProgramNode(programName, blockNode);
            Eat(TokenTypes.Dot);
            return programNode;
        }

        private CompoundNode CompoundStatement()
        {
            Eat(TokenTypes.Begin);
            var nodes = StatementList();
            Eat(TokenTypes.End);
            return new CompoundNode(nodes);
        }

        private AstNode[] StatementList()
        {
            var node = Statement();

            var results = new List<AstNode> { node };

            while (_currentToken.Type == TokenTypes.Semi)
            {
                Eat(TokenTypes.Semi);
                results.Add(Statement());
            }

            if (_currentToken.Type == TokenTypes.Id)
                throw new Exception("wat");

            return results.ToArray();
        }

        private AstNode Statement()
        {
            if (_currentToken.Type == TokenTypes.Begin)
                return CompoundStatement();
            else if (_currentToken.Type == TokenTypes.Id)
                return AssignmentStatement();
            else
                return Empty();
            throw new Exception("bad statement");
        }

        private AssignNode AssignmentStatement()
        {
            var left = Variable();
            Eat(TokenTypes.Assign);
            var right = Expression();
            return new AssignNode(left, right);
        }

        private VariableNode Variable()
        {
            var node = new VariableNode(_currentToken);
            Eat(TokenTypes.Id);
            return node;
        }

        private AstNode Empty() => new NoOpNode();

        private BlockNode Block()
        {
            var declarationNodes = Declarations();
            var compoundStatementNode = CompoundStatement();
            return new BlockNode(declarationNodes, compoundStatementNode);
        }

        private DeclarationNode[] Declarations()
        {
            var declarations = new List<DeclarationNode>();
            if (_currentToken.Type == TokenTypes.Variable)
            {
                Eat(_currentToken.Type);
                while (_currentToken.Type == TokenTypes.Id)
                {
                    declarations.AddRange(VariableDeclaration());
                    Eat(TokenTypes.Semi);
                }
            }

            while (_currentToken.Type == TokenTypes.Procedure)
            {
                Eat(TokenTypes.Procedure);
                var procedureName = _currentToken.Value.As<string>();
                Eat(TokenTypes.Id);
                Eat(TokenTypes.Semi);
                var blockNode = Block();
                declarations.Add(new ProcedureDeclarationNode(procedureName, blockNode));
                Eat(TokenTypes.Semi);
            }

            return declarations.ToArray();
        }

        private VariableDeclarationNode[] VariableDeclaration()
        {
            var variables = new List<VariableNode> { new VariableNode(_currentToken) };
            Eat(TokenTypes.Id);

            while (_currentToken.Type == TokenTypes.Comma)
            {
                Eat(TokenTypes.Comma);
                variables.Add(new VariableNode(_currentToken));
                Eat(TokenTypes.Id);
            }

            Eat(TokenTypes.Colon);

            var typeNode = TypeSpec();
            var declarations = new List<VariableDeclarationNode>();
            foreach (var variable in variables)
                declarations.Add(new VariableDeclarationNode(variable, typeNode));
            return declarations.ToArray();
        }

        private TypeNode TypeSpec()
        {
            var token = _currentToken;
            var node = new TypeNode(token);
            Eat(token.Type);
            return node;
        }
    }
}