using JavaSSTCompiler.Compiler.Builder.Attributes;
using JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.Structs
{
  public class MethodInfo : AbstractSerializable
  {
    [Field(0, typeof(ushort))]
    public ushort AccessFlags { get; init; }

    [Field(1, typeof(ushort))]
    public ushort NameIndex { get; init; }

    [Field(2, typeof(ushort))]
    public ushort DescriptorIndex { get; init; }

    [Field(3, typeof(AbstractSerializable), PrefixLenType = typeof(ushort))]
    public AbstractAttribute[] Attributes { get; private set; }
    
    
    public MethodInfo(AccessFlags accessFlags, Utf8Info nameInfo, Utf8Info descriptorInfo)
    {
      AccessFlags = (ushort)accessFlags;
      NameIndex = nameInfo.Index;
      DescriptorIndex = descriptorInfo.Index;
    }

    public void AddAttribute(AbstractAttribute attribute)
    {
      if (Attributes == null)
      {
        Attributes = new AbstractAttribute[1];
        Attributes[0] = attribute;
      }
      else
      {
        var newAttributes = new AbstractAttribute[Attributes.Length + 1];
        for (int i = 0; i < Attributes.Length; i++)
        {
          newAttributes[i] = Attributes[i];
        }
        newAttributes[^1] = attribute;
        Attributes = newAttributes;
      }
    }
  }
}
