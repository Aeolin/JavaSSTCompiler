using JavaSSTCompiler.Compiler.Builder.ConstantPool;
using JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos;
using JavaSSTCompiler.Compiler.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.Attributes
{
    public class LocalVariableTableEntry : AbstractSerializable
  {
    [Field(0)]
    public ushort StartPC { get; init; }
    
    [Field(1)]
    public ushort Length { get; init; }
    
    [Field(2)]
    public ushort NameIndex { get; init; }
    
    [Field(3)]
    public ushort DescriptorIndex { get; init; }
    
    [Field(4)]
    public ushort Index { get; init; }

    public LocalVariableTableEntry(ConstantPool.ConstantPool pool, string name, string type, ushort index, ushort startPC, ushort length)
    {
      NameIndex = pool.Utf8Info(name).Index;
      DescriptorIndex = pool.Utf8Info(new FieldDescriptorBuilder().WithType(type).Build()).Index;
      Index = index;
      StartPC = startPC;
      Length = length;
    }
  }
}
