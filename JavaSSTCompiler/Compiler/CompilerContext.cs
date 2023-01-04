using JavaSST.Compiler.Models;
using JavaSST.JavaStructs;
using JavaSST.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Method = JavaSST.Compiler.Models.Method;

namespace JavaSST.Compiler
{
  public class CompilerContext
  {
    public CompilerContext(BinaryWriter ouput)
    {
      Writer = ouput;
    }

    public ushort ConstantPoolCounter { get; set; } = 1;
    public Dictionary<int, ConstantPoolInfo> ConstantPool { get; init; } = new Dictionary<int, ConstantPoolInfo>();
    public Dictionary<string, Variable> Variables { get; init; } = new Dictionary<string, Variable>();
    public Dictionary<string, Method> Methods { get; init; } = new Dictionary<string, Method>();

    public ConstantPoolInfo CreateConstantPoolInfo(ConstantPoolTag tag)
    {
      var info = new ConstantPoolInfo(ConstantPoolCounter++, tag);
      ConstantPool.Add(info.Index, info);
      return info;
    }

    public Variable MakeVariable(ConstantPoolInfo classInfo, DynamicField field, CompilerContext ctx)
    {
      var info = ConstantPoolInfo.FieldRefInfo(classInfo, field, ctx);
      var variable = new Variable(field.Identifier, "I", true, info);
      Variables.Add(variable.Name, variable);
      return variable;
    }

    public Variable MakeVariable(ConstantPoolInfo classInfo, FinalField field, CompilerContext ctx)
    {
      var info = ConstantPoolInfo.FieldRefInfo(classInfo, field, ctx);
      var variable = new Variable(field.Identifier, field.Type.Value, false, info);
      Variables.Add(variable.Name, variable);
      return variable;
    }

    public Method MakeMethod(ConstantPoolInfo classInfo, JavaSST.Parser.Models.Method method, CompilerContext ctx)
    {
      var info = ConstantPoolInfo.MethodRefInfo(classInfo, method, ctx);
      var result = new Method(method.Identifier, method.ReturnType.Type == Tokenizer.TokenType.Void ? "V" : "I", info);
      Methods.Add(result.Name, result);
      return result;
    }


    public BinaryWriter Writer { get; init; }
  }
}
