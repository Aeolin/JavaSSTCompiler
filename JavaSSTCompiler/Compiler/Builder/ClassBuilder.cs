using JavaSST.Compiler.Models;
using JavaSSTCompiler.Compiler.Builder.Attributes;
using JavaSSTCompiler.Compiler.Builder.ByteCode;
using JavaSSTCompiler.Compiler.Builder.ConstantPool;
using JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos;
using JavaSSTCompiler.Compiler.Builder.Structs;
using JavaSSTCompiler.Compiler.Models;
using JavaSSTCompiler.Compiler.Utils;
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
    public string CodeFile { get; private set; }
    public AccessFlags AccessFlags { get; private set; } = AccessFlags.Public;
    
    private ClassInfo _thisClass;
    public ClassInfo ThisClassInfo => _thisClass;
    
    private ClassInfo _superClass;
    public ClassInfo SuperClassInfo => _superClass;

    private ConstantPool.ConstantPool _constantPool = new ConstantPool.ConstantPool();
    public ConstantPool.ConstantPool ConstantPool => _constantPool;

    private List<FieldInfo> _fields = new List<FieldInfo>();
    private Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();
    
    public static ClassBuilder Create() => new ClassBuilder();

    private ClassBuilder() 
    {
    }

    public RefInfo GetMethodRef(string methodName)
    {
      var method = _methods[methodName];
      return _constantPool.MethodRefInfo(_thisClass, _constantPool.GetInfo<Utf8Info>(method.NameIndex), _constantPool.GetInfo<Utf8Info>(method.DescriptorIndex));
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
      _methods.Add(name, new MethodInfo(flags, _constantPool.Utf8Info(name), _constantPool.Utf8Info(builder.Build())));
      return this;
    }

    public ClassBuilder MethodBody(string name, string returnType, IEnumerable<(string type, string name)> parameters, Action<ByteCodeBuilder> builderAction)
    {
      var method = new Method(name, returnType) { Parameters = parameters == null ? new Parameter[0] : parameters.Select(x => new Parameter { Name = x.name, Type = x.type }).ToArray() };
      var builder = new ByteCodeBuilder(this, method);
      builderAction(builder);
      _methods[name].AddAttributes(builder.Build().ToArray());
      return this;
    }

    public ClassBuilder WithCodeFile(string codeFile)
    {
      CodeFile = codeFile;
      return this;
    }

    private IEnumerable<AbstractAttribute> getAttributes()
    {
      if (CodeFile != null) 
        yield return new SourceFileAttribute(_constantPool, CodeFile); 
    }

    public byte[] Compile()
    {
      using var mem = new MemoryStream();
      using var writer = new BigEndianBinaryWriter(mem);
      var attributes = getAttributes().ToArray();         // generate attributes before constant pool is saved      
      writer.Write(0xCAFEBABE);
      writer.Write((ushort)0);
      writer.Write((ushort)51);
      writer.Write(_constantPool.ToBytes());
      writer.Write((ushort)AccessFlags);
      writer.Write(_thisClass.Index);
      writer.Write(_superClass.Index);
      writer.Write((ushort)0); // no interfaces
      writer.Write((ushort)_fields.Count);
      foreach (var field in _fields)
        writer.Write(field.ToBytes());
      writer.Write((ushort)_methods.Count);
      foreach (var method in _methods.Values)
        writer.Write(method.ToBytes());

      writer.Write((ushort)attributes.Length);
      foreach (var attribute in attributes)
        writer.Write(attribute.ToBytes());
      
      return mem.ToArray();
    }
  }
}
