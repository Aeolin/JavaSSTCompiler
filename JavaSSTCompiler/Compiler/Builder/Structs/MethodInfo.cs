using JavaSSTCompiler.Compiler.Builder.Attributes;
using JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos;
using JavaSSTCompiler.Compiler.Utils;
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
    
    // only for internal compiler usage
    public string Name { get; set; }

    public MethodInfo(AccessFlags accessFlags, Utf8Info nameInfo, Utf8Info descriptorInfo)
    {
      AccessFlags = (ushort)accessFlags;
      NameIndex = nameInfo.Index;
      DescriptorIndex = descriptorInfo.Index;
    }


    public void AddAttributes(params AbstractAttribute[] attributes)
    {
      if (Attributes == null)
      {
        Attributes = attributes;
      }
      else
      {
        var newAttributes = new AbstractAttribute[Attributes.Length + attributes.Length];
        Array.Copy(Attributes, newAttributes, Attributes.Length);
        Array.Copy(attributes, 0, newAttributes, Attributes.Length, attributes.Length);
      }
    }
  }
}
