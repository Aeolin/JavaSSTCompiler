using JavaSST.Compiler;
using JavaSST.Compiler.Models;
using JavaSST.Parser.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Method = JavaSST.Parser.Models.Method;

namespace JavaSST.JavaStructs
{
  public class ConstantPoolInfo
  {
    public ushort Index { get; init; }
    public ConstantPoolTag Tag { get; init; }

    public ConstantPoolInfo(ushort index, ConstantPoolTag tag)
    {
      Index=index;
      Tag=tag;
    }

    public byte[] Info { get; set; }

    static byte[] bigEndianUshorts(params ushort[] nums)
    {
      var bytes = new byte[nums.Length * 2];
      for (int i = 0; i < nums.Length; i++)
      {
        var off = i*2;
        bytes[off+0] = (byte)(nums[i] >> 8);
        bytes[off+1] = (byte)(nums[i] & 0xFF);
      }

      return bytes;
    }

    public static ConstantPoolInfo ClassInfo(Class clazz, CompilerContext ctx)
    {
      var name = Utf8Info(clazz.Identifier, ctx);
      var info = ctx.CreateConstantPoolInfo(ConstantPoolTag.Class);
      info.Info = BitConverter.GetBytes(name.Index).Reverse().ToArray();
      return info;
    }

    public static ConstantPoolInfo Utf8Info(string value, CompilerContext ctx)
    {
      var info = ctx.CreateConstantPoolInfo(ConstantPoolTag.Utf8);
      var bytes = Encoding.ASCII.GetBytes(value); // only works because java special modified utf8 and javasst only allowing ascii characters by design
      var len = BitConverter.GetBytes((ushort)bytes.Length).Reverse().ToArray();
      info.Info = len.Concat(bytes).ToArray();
      return info;
    }

    public static ConstantPoolInfo NameAndTypeInfo(string name, string type, CompilerContext ctx)
    {
      var nameInfo = Utf8Info(name, ctx);
      var typeInfo = Utf8Info(type, ctx);
      var info = ctx.CreateConstantPoolInfo(ConstantPoolTag.NameAndType);
      info.Info = bigEndianUshorts(nameInfo.Index, typeInfo.Index);
      return info;
    }

    public static ConstantPoolInfo FieldRefInfo(ConstantPoolInfo classInfo, DynamicField field, CompilerContext ctx)
    {
      var info = ctx.CreateConstantPoolInfo(ConstantPoolTag.Fieldref);
      var typeInfo = NameAndTypeInfo(field.Identifier, "I", ctx);
      info.Info = bigEndianUshorts(classInfo.Index, typeInfo.Index);
      return info;
    }

    public static ConstantPoolInfo FieldRefInfo(ConstantPoolInfo classInfo, FinalField field, CompilerContext ctx)
    {
      var info = ctx.CreateConstantPoolInfo(ConstantPoolTag.Fieldref);
      var typeInfo = NameAndTypeInfo(field.Identifier, "I", ctx);
      info.Info = bigEndianUshorts(classInfo.Index, typeInfo.Index);
      return info;
    }

    public static string GetMethodDescriptor(Method method)
    {
      return $"({new string('I', method.Parameters.Length)}){(method.ReturnType.Type == Tokenizer.TokenType.Void ? "V" : "I")}";
    }

    public static ConstantPoolInfo MethodRefInfo(ConstantPoolInfo classInfo, Method method, CompilerContext ctx)
    {
      var info = ctx.CreateConstantPoolInfo(ConstantPoolTag.Methodref);
      var typeInfo = NameAndTypeInfo(method.Identifier, GetMethodDescriptor(method), ctx);
      info.Info = bigEndianUshorts(classInfo.Index, typeInfo.Index);
      return info;
    }

  }

}
