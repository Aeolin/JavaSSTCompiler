using JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JavaSSTCompiler.Compiler.Builder.ConstantPool.DescriptorBuilder;

namespace JavaSSTCompiler.Compiler.Builder.ConstantPool
{
  public class ConstantPool
  {
    private List<AbstractConstantPoolInfo> _infos = new List<AbstractConstantPoolInfo>();
    private ushort _nextIndex = 1;

    public ConstantPool()
    {
      _infos.Add(null);
    }

    public ushort NextIndex()
    {
      return _nextIndex++;
    }

    public int Add(AbstractConstantPoolInfo info)
    {
      _infos.Add(info);
      return _infos.Count - 1;
    }

    public byte[] ToBytes()
    {
      using var stream = new MemoryStream();
      stream.Write(BitConverter.GetBytes((ushort)_infos.Count).Reverse().ToArray());
      foreach (var info in _infos.OrderBy(x => x.Index))
        stream.Write(info.ToBytes());

      return stream.ToArray();
    }

    public Utf8Info Utf8Info(string value)
    {
      var info = _infos.OfType<Utf8Info>().FirstOrDefault(x => x.Value == value);
      if (info == null)
      {
        info = new Utf8Info(value, NextIndex());
        _infos.Add(info);
      }

      return info;
    }

    public ClassInfo ClassInfo(string name)
    {
      var nameInfo = Utf8Info(name);
      var info = _infos.OfType<ClassInfo>().FirstOrDefault(x => x.NameIndex == nameInfo.Index);
      if (info == null)
      {
        info = new ClassInfo(nameInfo, NextIndex());
        _infos.Add(info);
      }

      return info;
    }

    public RefInfo MethodRefInfo(ClassInfo classInfo, string name, string returnType, IEnumerable<string> parameters) => MethodRefInfo(classInfo, name, x => x.WithReturnType(returnType).WithParameters(parameters));
    public RefInfo MethodRefInfo(ClassInfo classInfo, string name, Action<MethodDescriptorBuilder> descriptorAction)
    {
      var builder = new MethodDescriptorBuilder();
      descriptorAction(builder);
      var descriptor = builder.Build();
      var nani = NameAndTypeIndex(name, descriptor);
      var info = _infos.OfType<RefInfo>()
        .Where(x => x.Tag == ConstantPoolTag.MethodRef)
        .FirstOrDefault(x => x.NameAndTypeIndex == nani.Index && x.ClassIndex == classInfo.Index);

      if(info == null)
      {
        info = new RefInfo(classInfo, nani, ConstantPoolTag.MethodRef, NextIndex());
        _infos.Add(info);
      }

      return info;
    }

    public RefInfo FieldRefInfo(ClassInfo classInfo, string name, string type)
    {
      var descriptor = new FieldDescriptorBuilder().WithType(type).Build();
      var nani = NameAndTypeIndex(name, descriptor);
      var info = _infos.OfType<RefInfo>()
        .Where(x => x.Tag == ConstantPoolTag.FieldRef)
        .FirstOrDefault(x => x.NameAndTypeIndex == nani.Index && x.ClassIndex == classInfo.Index);

      if (info == null)
      {
        info = new RefInfo(classInfo, nani, ConstantPoolTag.FieldRef, NextIndex());
        _infos.Add(info);
      }

      return info;
    }

    public NameAndTypeIndex NameAndTypeIndex(string name, string type)
    {
      var nameInfo = Utf8Info(name);
      var typeInfo = Utf8Info(type);
      var info = _infos.OfType<NameAndTypeIndex>().FirstOrDefault(x => x.TypeIndex == typeInfo.Index && x.NameIndex == nameInfo.Index);
      if (info == null)
      {
        info = new NameAndTypeIndex(nameInfo, typeInfo, NextIndex());
        _infos.Add(info);
      }

      return info;
    }

    public IntInfo IntInfo(int value)
    {
      var info = _infos.OfType<IntInfo>().FirstOrDefault(x => x.Value == value);
      if(info == null)
      {
        info = new IntInfo(value, NextIndex());
        _infos.Add(info);
      }

      return info;
    }

    public StringInfo StringInfo(string value)
    {
      var valueInfo = Utf8Info(value);
      var info = _infos.OfType<StringInfo>().FirstOrDefault(x => x.StringIndex == valueInfo.Index);
      if(info == null)
      {
        info = new StringInfo(valueInfo, NextIndex());
        _infos.Add(info);
      }

      return info;
    }

  }
}
