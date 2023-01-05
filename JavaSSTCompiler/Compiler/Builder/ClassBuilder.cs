using JavaSST.Compiler.Models;
using JavaSSTCompiler.Compiler.Builder.Attributes;
using JavaSSTCompiler.Compiler.Builder.ConstantPool;
using JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos;
using JavaSSTCompiler.Compiler.Builder.Structs;
using JavaSSTCompiler.Compiler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder
{
  public class ClassBuilder
  {
    public string Name { get; private set; }
    public string SuperClass { get; private set; }
    public AccessFlags AccessFlags { get; private set; } = AccessFlags.Public;
    
    private ClassInfo _thisClass;
    private ClassInfo _superClass;

    private ConstantPool.ConstantPool _constantPool = new ConstantPool.ConstantPool();

    private List<FieldInfo> _fields = new List<FieldInfo>();
    private List<MethodInfo> _methods = new List<MethodInfo>();
    
    public static ClassBuilder Create() => new ClassBuilder();

    private ClassBuilder() 
    {
    }

    public ClassBuilder WithName(string name)
    {
      Name = name;
      _thisClass = _constantPool.ClassInfo(name);
      return this;
    }

    public ClassBuilder WithSuperClass(string superClass = "java/lang/Object")
    {
      SuperClass = superClass;
      _superClass = _constantPool.ClassInfo(superClass);
      return this;
    }

    public ClassBuilder WithAccessFlags(AccessFlags flags)
    {
      this.AccessFlags = flags;
      return this;
    }

    public ClassBuilder WithField(AccessFlags flags, string name, string type) => WithField(flags, name, x => x.WithType(type));
    public ClassBuilder WithField(AccessFlags flags, string name, Action<FieldDescriptorBuilder> builderAction)
    {
      var builder = new FieldDescriptorBuilder();
      builderAction(builder);
      _fields.Add(new FieldInfo(flags, _constantPool.Utf8Info(name), _constantPool.Utf8Info(builder.Build())));
      return this;
    }

    public ClassBuilder WithMethod(AccessFlags flags, string name, string returnType, IEnumerable<string> types) => WithMethod(flags, name, x => x.WithReturnType(returnType).WithParameters(types));
    public ClassBuilder WithMethod(AccessFlags flags, string name, Action<MethodDescriptorBuilder> builderAction)
    {
      var builder = new MethodDescriptorBuilder();
      builderAction(builder);
      _methods.Add(new MethodInfo(flags, _constantPool.Utf8Info(name), _constantPool.Utf8Info(builder.Build())));
      return this;
    }

    public ClassBuilder MethodBody(string name, string returnType, IEnumerable<(string type, string name)> parameters, Action<CodeBuilder> builderAction)
    {
      var method = new Method(name, returnType) { Parameters = parameters.Select(x => new Parameter { Name = x.name, Type = x.type }).ToArray() };
      var builder = new CodeBuilder(_constantPool, method, _thisClass);
      builderAction(builder);
      var code = builder.Build();
      var attribute = new CodeAttribute(_constantPool) { Code = code };
    }


    public byte[] Compile()
    {
      using var mem = new MemoryStream();
      using var writer = new ConstantPoolBinaryWriter(mem);
      writer.Write(0xBEBAFECA);
      writer.Write((ushort)51);
      writer.Write((ushort)0);
      writer.Write(_constantPool.ToBytes());
      writer.Write((ushort)AccessFlags);
      writer.Write(_thisClass.Index);
      writer.Write(_superClass.Index);
      writer.Write((ushort)0); // no interfaces
      writer.Write((ushort)_fields.Count);
      foreach (var field in _fields)
        writer.Write(field.ToBytes());
      writer.Write((ushort)_methods.Count);
      foreach (var method in _methods)
        writer.Write(method.ToBytes());
      writer.Write((ushort)0); // no attributes
      return mem.ToArray();
    }
  }
}
