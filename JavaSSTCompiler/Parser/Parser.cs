using JavaSST.Parser.Models;
using JavaSST.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.Parser
{
  public class Parser
  {
    public Parser()
    {

    }

    /// <summary>
    /// Takes a stream of tokens and arranges them into a Model of type Class
    /// </summary>
    /// <param name="tokens">A stream of tokens</param>
    /// <returns>A Class file representing the parsed tokens in a structure as specified by the JavaSST Bakus Naur Form</returns>
    public Class Parse(IEnumerable<Token> tokens)
    {
      var ctx = new ParserContext(tokens.Where(x => x.Type != TokenType.Comment && x.Type != TokenType.Whitespace)); // ignore all whitespaces and comments
      return parseClass(ctx);
    }

    /// <summary>
    /// Parses the main Class file
    /// </summary>
    /// <param name="ctx">ParserContext</param>
    /// <returns>parsed Class</returns>
    /// <exception cref="ParserException">Throws exception if tokens are left after class was parsed</exception>
    private Class parseClass(ParserContext ctx)
    {
      ctx.NextAndValidate("Expected Class definition", TokenType.Class);
      var clazz = new Class(ctx);
      ctx.NextAndValidate("Expected { after class definition", TokenType.LCurly);
      clazz.FinalFields = parseFinalFields(ctx).ToArray();
      clazz.DynamicFields = parseDynamicFields(ctx).ToArray();
      clazz.Methods = parseMethods(ctx).ToArray();
      ctx.NextAndValidate("Expected } after class definition", TokenType.RCurly);
      if (ctx.TokensLeft > 0)
        throw new ParserException(ctx.CurrentToken, "Expected no more tokens");

      return clazz;
    }

    /// <summary>
    /// Parse the final fields speciefied at the start of a JavaSST class
    /// </summary>
    /// <param name="ctx">ParserContext</param>
    /// <returns>An IEnumerable of parsed FinalField models</returns>
    /// <exception cref="ParserException">Throws exception if unexpected structures occur</exception>
    private IEnumerable<FinalField> parseFinalFields(ParserContext ctx)
    {
      while (ctx.Is(TokenType.Final))
      {
        ctx.NextToken();
        var type = ctx.NextAndValidate("expected field type", TokenType.Type);
        var field = new FinalField(ctx);
        field.Type = type;
        ctx.NextAndValidate("expected assignment operator", TokenType.Assign);
        field.Expression = parseExpression(ctx);
        ctx.NextAndValidate("expected semicolon at the end of final field declaration", TokenType.Semicolon);
        yield return field;
      }
    }

    /// <summary>
    /// Parse the dynamic fields speciefied at the start of a JavaSST class
    /// </summary>
    /// <param name="ctx">ParserContext</param>
    /// <returns>An IEnumerable of parsed DynamicFields models</returns>
    /// <exception cref="ParserException">Throws exception if unexpected structures occur</exception>
    private IEnumerable<DynamicField> parseDynamicFields(ParserContext ctx)
    {
      while (ctx.Is(TokenType.Type))
      {
        var type = ctx.NextToken();
        var field = new DynamicField(ctx);
        field.Type = type;
        ctx.NextAndValidate("expected semicolon at the end of dynamic field declaration", TokenType.Semicolon);
        yield return field;
      }
    }

    /// <summary>
    /// Parse all methods specified inside a JavaSST class
    /// </summary>
    /// <param name="ctx">ParserContext</param>
    /// <returns>An IEnumerable of parsed Method models</returns>
    /// <exception cref="ParserException">Throws exception if unexpected structures occur</exception>
    private IEnumerable<Method> parseMethods(ParserContext ctx)
    {
      while (ctx.Is(TokenType.Public)) // all methods start with the public keyword
      {
        ctx.NextToken();
        var type = ctx.NextAndValidate("expected method return type", TokenType.Void, TokenType.Type);
        var method = new Method(ctx, type);
        ctx.NextAndValidate("expected open paranthesis", TokenType.LParen);
        method.Parameters = parseFormalParameters(ctx).ToArray();
        ctx.NextAndValidate("expected closed paranthesis", TokenType.RParen);
        ctx.NextAndValidate("expected open curly bracket", TokenType.LCurly);
        method.Body = parseMethodBody(ctx);
        ctx.NextAndValidate("expected closed curly bracket", TokenType.RCurly);
        yield return method;
      }
    }

    /// <summary>
    /// Parses the body of a method
    /// </summary>
    /// <param name="ctx">ParserContext</param>
    /// <returns>A MethodBody model containing parsed information of the local fields and statements which make up the method</returns>
    /// <exception cref="ParserException">Throws exception if unexpected structures occur</exception>
    private MethodBody parseMethodBody(ParserContext ctx)
    {
      var body = new MethodBody();
      body.LocalVariables = parseDynamicFields(ctx).ToArray();
      body.Statements = parseStatements(ctx).ToArray();
      return body;
    }

    /// <summary>
    /// Parses a sequence of statement 
    /// </summary>
    /// <param name="ctx">ParseContext</param>
    /// <returns>IEnumerable of Statements</returns>
    /// <exception cref="ParserException">Throws exception if unexpected structures occur</exception>
    private IEnumerable<IStatement> parseStatements(ParserContext ctx)
    {
      // statement sequence must contain at least one sequence
      do
      {
        yield return parseStatement(ctx);
      } while (ctx.Is(TokenType.RCurly) == false);
    }

    /// <summary>
    /// Parses a single statement
    /// </summary>
    /// <param name="ctx">ParseContext</param>
    /// <returns></returns>
    /// <exception cref="ParserException">Throws exception if unexpected structures occur</exception>
    private IStatement parseStatement(ParserContext ctx)
    {
      if (ctx.Is(TokenType.Identifier)) // Assignement and ProcedureCall Statements start with an identifier
      {
        var identifier = ctx.NextToken();
        if (ctx.Is(TokenType.Assign))
        {
          ctx.NextAndValidate("expected assignment operator", TokenType.Assign);
          var assign = new Assignment(identifier);
          assign.Expression = parseExpression(ctx);
          ctx.NextAndValidate("expected semicolon at the end of assignment", TokenType.Semicolon);
          return assign;
        }
        else
        {
          var procedure = parseProcedureCall(identifier, ctx);
          ctx.NextAndValidate("expected semicolon at the end of procedure call", TokenType.Semicolon);
          return procedure;
        }
      }

      if (ctx.Is(TokenType.If))
        return parseIfStatement(ctx);

      if (ctx.Is(TokenType.While))
        return parseWhileStatement(ctx);

      if (ctx.Is(TokenType.Return))
        return parseReturnStatement(ctx);

      throw new ParserException(ctx.CurrentToken, "expected statement");
    }

    private IfStatement parseIfStatement(ParserContext ctx)
    {
      var statement = new IfStatement();
      ctx.NextAndValidate("expected if token", TokenType.If);
      ctx.NextAndValidate("expected open paranthesis before condition", TokenType.LParen);
      statement.Condition = parseExpression(ctx);
      ctx.NextAndValidate("expected closed paranthesis after condition", TokenType.RParen);
      ctx.NextAndValidate("expected open curly bracket before then", TokenType.LCurly);
      statement.ThenStatements = parseStatements(ctx).ToArray();
      ctx.NextAndValidate("expected open curly bracket after then", TokenType.RCurly);
      ctx.NextAndValidate("expected else after then block", TokenType.Else);
      ctx.NextAndValidate("expected open curly bracket before else", TokenType.LCurly);
      statement.ElseStatements = parseStatements(ctx).ToArray();
      ctx.NextAndValidate("expected closed curly bracket after else", TokenType.RCurly);
      return statement;
    }

    private WhileStatement parseWhileStatement(ParserContext ctx)
    {
      var statement = new WhileStatement();
      ctx.NextAndValidate("expected while token", TokenType.While);
      ctx.NextAndValidate("expected open paranthesis before condition", TokenType.LParen);
      statement.Condition = parseExpression(ctx);
      ctx.NextAndValidate("expected closed paranthesis after condition", TokenType.RParen);
      ctx.NextAndValidate("expected open curly bracket before while body", TokenType.LCurly);
      statement.Statements = parseStatements(ctx).ToArray();
      ctx.NextAndValidate("expected closed curly bracket after while body", TokenType.RCurly);
      return statement;
    }

    private ReturnStatement parseReturnStatement(ParserContext ctx)
    {
      var statement = new ReturnStatement();
      ctx.NextAndValidate("expected return token", TokenType.Return);
      if (ctx.Is(TokenType.Semicolon) == false)
        statement.Expression = parseSimpleExpression(ctx);

      ctx.NextAndValidate("expected semicolon at the end of return statement", TokenType.Semicolon);
      return statement;
    }

    private IEnumerable<FormalParameter> parseFormalParameters(ParserContext ctx)
    {
      bool first = true;
      while (ctx.Is(TokenType.Type, TokenType.Comma))
      {
        if (first == false)
          ctx.NextAndValidate("expected comma", TokenType.Comma);

        first = false;
        var type = ctx.NextAndValidate("expected parameter type", TokenType.Type);
        var parameter = new FormalParameter(ctx, type);
        yield return parameter;
      }
    }

    private Token parseComparison(ParserContext ctx) => ctx.NextOptional(TokenType.Equal, TokenType.LessOrEqual, TokenType.LessThan, TokenType.GreaterThan, TokenType.GreaterOrEqual);

    private Expression parseExpression(ParserContext ctx)
    {
      var expression = new Expression();
      expression.Left = parseSimpleExpression(ctx);
      expression.Comparison = parseComparison(ctx)?.Type;
      if (expression.Comparison.HasValue)
        expression.Right = parseSimpleExpression(ctx);
      return expression;
    }

    private SimpleExpression parseSimpleExpression(ParserContext ctx)
    {
      var expression = new SimpleExpression();
      expression.Term = parseTerm(ctx);
      while (ctx.Is(TokenType.Plus, TokenType.Minus))
      {
        var operation = ctx.NextToken();
        var term = parseTerm(ctx);
        expression.Terms.Add((operation.Type, term));
      }

      return expression;
    }

    private Term parseTerm(ParserContext ctx)
    {
      var term = new Term();
      term.Factor = parseFactor(ctx);
      while (ctx.Is(TokenType.Multiply, TokenType.Divide))
      {
        var operation = ctx.NextToken();
        var factor = parseFactor(ctx);
        term.Factors.Add((operation.Type, factor));
      }
      return term;
    }

    private IFactor parseFactor(ParserContext ctx)
    {
      if (ctx.Is(TokenType.Identifier))
      {
        var token = ctx.NextAndValidate("expected identifier", TokenType.Identifier);
        if (ctx.Is(TokenType.LParen))
        {
          return parseProcedureCall(token, ctx);
        }
        else
        {
          return new Identifier(token.Value);
        }
      }

      if (ctx.Is(TokenType.Number))
        return parseNumber(ctx);

      if (ctx.Is(TokenType.LParen))
      {
        ctx.NextToken();
        var expr = parseExpression(ctx);
        ctx.NextAndValidate("expected closing parenthesis", TokenType.RParen);
        return expr;
      }

      throw new ParserException(ctx.CurrentToken, "expected factor");
    }

    private ProcedureCall parseProcedureCall(Token identifier, ParserContext ctx)
    {
      var call = new ProcedureCall(identifier.Value);
      ctx.NextAndValidate("expected open paranthesis", TokenType.LParen);
      bool first = true;
      while (ctx.Is(TokenType.RParen) == false)
      {
        if (first == false)
          ctx.NextAndValidate("expected comma to separate arguments", TokenType.Comma);

        first = false;
        call.Arguments.Add(parseExpression(ctx));
      }
      ctx.NextAndValidate("expected closed paranthesis", TokenType.RParen);
      /* works only in a JavaSST context where every method parameter must be an int
         requires more intesive reworking to figure out method signature by analysing
         the parsed expressions and their types */ 
      call.MethodSignature = $"{identifier.Value}({string.Join(":", call.Arguments.Select(x => "int"))})"; 
      return call;
    }

    private Identifier parserIdentifier(ParserContext ctx) => new Identifier(ctx);

    private Number parseNumber(ParserContext ctx) => new Number(ctx);

  }
}
