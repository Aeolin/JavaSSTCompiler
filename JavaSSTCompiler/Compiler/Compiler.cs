﻿using JavaSST.Parser.Models;
using JavaSST.Tokenizer;
using JavaSSTCompiler.Compiler.Builder;
using JavaSSTCompiler.Compiler.Builder.ByteCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JavaSSTCompiler.Compiler.Builder.ByteCode.ByteCodeBuilder;

namespace JavaSST.Compiler
{
  public class Compiler
  {
    public Compiler()
    {

    }

    public byte[] Compile(Class clazz, string codeFile = null)
    {
      var builder = ClassBuilder.Create();
      compileClass(builder, clazz);
      if (codeFile != null)
        builder.WithCodeFile(codeFile);
      
      var data = builder.Compile();
      return data;
    }

    private void compileClass(ClassBuilder builder, Class clazz)
    {
      builder
        .WithName(clazz.Identifier)
        .WithSuperClass()
        .WithAccessFlags(AccessFlags.Public);

      foreach (var field in clazz.FinalFields)
        builder.WithField(AccessFlags.Public | AccessFlags.Final, field.Identifier, field.Type.Value);

      foreach (var field in clazz.DynamicFields)
        builder.WithField(AccessFlags.Public, field.Identifier, field.Type.Value);

      foreach (var method in clazz.Methods)
        builder.WithMethod(AccessFlags.Public, method.Identifier, method.Signature, method.ReturnType.Value, method.Parameters.Select(x => x.Type.Value));

      builder.WithMethod(AccessFlags.Public, "<init>", "<init>()", "void", null);
      builder.MethodBody("<init>()", "void", null, cb =>
      {
        cb.Aload(0);
        cb.InvokeSpecial(builder.ConstantPool.MethodRefInfo(builder.SuperClassInfo, "<init>", "void", null));
        foreach (var field in clazz.FinalFields)
        {
          cb.Aload(0);
          compileExpression(cb, field.Expression);
          cb.StoreIntVariable(field.Identifier);
        }
        cb.Return();
      });

      foreach (var method in clazz.Methods)
      {
        builder.MethodBody(method.Signature, method.ReturnType.Value, method.Parameters.Select(x => (x.Type.Value, x.Identifier)), cb =>
        {
          var body = method.Body;
          foreach (var localVar in body.LocalVariables)
            cb.DefineLocal(localVar.Identifier);

          foreach (var statement in body.Statements)
            compileStatement(cb, statement);

          if (body.Statements.Last() is not ReturnStatement)
            cb.Return();
        });
      }
    }

    private void compileStatement(ByteCodeBuilder builder, IStatement statement)
    {
      if (statement is Assignment assignment)
        compileAssignment(builder, assignment);
      else if (statement is ProcedureCall procedureCall)
        compileProcedureCall(builder, procedureCall);
      else if (statement is IfStatement ifStatement)
        compileIfStatement(builder, ifStatement);
      else if (statement is WhileStatement whileStatement)
        compileWhileStatement(builder, whileStatement);
      else if (statement is ReturnStatement returnStatement)
        compileReturnStatement(builder, returnStatement);
      else
        throw new InvalidOperationException("Unknown statement");
    }

    private void compileReturnStatement(ByteCodeBuilder builder, ReturnStatement statement)
    {
      if (statement.Expression == null)
      {
        builder.Return();
      }
      else
      {
        compileSimpleExpression(builder, statement.Expression);
        builder.IReturn();
      }
    }

    private static readonly Dictionary<TokenType, CompareType> CMP_LOOKUP = new Dictionary<TokenType, CompareType>
    {
      { TokenType.LessThan, CompareType.LessThan },
      { TokenType.LessOrEqual, CompareType.LessThanOrEqual },
      { TokenType.GreaterThan, CompareType.GreaterThan },
      { TokenType.GreaterOrEqual, CompareType.GreaterThanOrEqual },
      { TokenType.Equal, CompareType.Equal },
    };

    private void compileWhileStatement(ByteCodeBuilder builder, WhileStatement whileStatement)
    {
      builder.Goto(out var comparisonMarker);
      var pos = builder.CurrentAddress;
      foreach (var statement in whileStatement.Statements)
        compileStatement(builder, statement);
      comparisonMarker.WriteRelative(builder.CurrentAddress);
      if (whileStatement.Condition.Right == null)
        throw new InvalidOperationException("Expected right side of condition inside if statement");

      compileSimpleExpression(builder, whileStatement.Condition.Left);
      compileSimpleExpression(builder, whileStatement.Condition.Right);
      builder.IfICmp(CMP_LOOKUP[whileStatement.Condition.Comparison.Value], pos);
    }

    private void compileIfStatement(ByteCodeBuilder builder, IfStatement ifStatement)
    {
      if (ifStatement.Condition.Right == null)
        throw new InvalidOperationException("Expected right side of condition inside if statement");

      compileSimpleExpression(builder, ifStatement.Condition.Left);
      compileSimpleExpression(builder, ifStatement.Condition.Right);
      builder.IfICmp(CMP_LOOKUP[ifStatement.Condition.Comparison.Value], out var elseMarker);

      foreach (var elseStatement in ifStatement.ElseStatements)
        compileStatement(builder, elseStatement);
      builder.Goto(out var thenMarker);
      elseMarker.WriteRelative(builder.CurrentAddress);

      foreach (var thenStatement in ifStatement.ThenStatements)
        compileStatement(builder, thenStatement);
      thenMarker.WriteRelative(builder.CurrentAddress);
    }



    private void compileAssignment(ByteCodeBuilder builder, Assignment assignment)
    {
      compileExpression(builder, assignment.Expression);
      builder.StoreIntVariable(assignment.Identifier);
    }

    private void compileExpression(ByteCodeBuilder builder, Expression expression)
    {
      compileSimpleExpression(builder, expression.Left);
      if (expression.Right != null)
        throw new InvalidOperationException("Expression can't have a comparison outside of an if or while statement");
    }

    private void compileSimpleExpression(ByteCodeBuilder builder, SimpleExpression expression)
    {
      compileTerm(builder, expression.Term);
      foreach (var term in expression.Terms)
      {
        compileTerm(builder, term.Term);
        if (term.Operator == Tokenizer.TokenType.Plus)
          builder.IAdd();
        else if (term.Operator == Tokenizer.TokenType.Minus)
          builder.ISub();
        else
          throw new InvalidOperationException("Invalid operator in expression");
      }
    }

    private void compileTerm(ByteCodeBuilder builder, Term term)
    {
      compileFactor(builder, term.Factor);
      foreach (var factor in term.Factors)
      {
        compileFactor(builder, factor.Factor);
        if (factor.Type == TokenType.Multiply)
          builder.IMul();
        else if (factor.Type == TokenType.Divide)
          builder.IDiv();
        else
          throw new InvalidOperationException("Invalid operator in expression");
      }
    }

    private void compileProcedureCall(ByteCodeBuilder builder, ProcedureCall call)
    {
      builder.Aload(0);
      foreach (var arg in call.Arguments)
        compileExpression(builder, arg);

      builder.InvokeVirtual(builder.ClassBuilder.GetMethodRef(call.MethodSignature));
    }

    private void compileFactor(ByteCodeBuilder builder, IFactor factor)
    {
      if (factor is Identifier variable)
        builder.LoadIntVariable(variable.Identifier);
      else if (factor is Number number)
        builder.ConstInt(number.Value);
      else if (factor is Expression expression)
        compileExpression(builder, expression);
      else if (factor is ProcedureCall call)
        compileProcedureCall(builder, call);
      else
        throw new InvalidOperationException("no factor found");
    }
  }
}