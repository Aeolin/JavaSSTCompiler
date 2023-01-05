using JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.Structs
{
  public class FieldInfo : AbstractSerializable
  {
    [Field(0)]
    public ushort AccessFlags { get; set; }
    [Field(1)]
    public ushort NameIndex { get; set; }
    [Field(2)]
    public ushort DescriptorIndex { get; set; }
    [Field(3)]
    public ushort AttributesCount => 0;

    public FieldInfo(AccessFlags accessFlags, Utf8Info nameInfo, Utf8Info descriptorInfo)
    {
      AccessFlags = (ushort)accessFlags;
      NameIndex = nameInfo.Index;
      DescriptorIndex = descriptorInfo.Index;
    }
  }
}
