using JavaSSTCompiler.Compiler.Builder.ConstantPool;
using JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.Attributes
{
  public class AbstractAttribute : AbstractSerializable
  {
    public ushort AttributeNameIndex { get; init; }


    public AbstractAttribute(Utf8Info nameInfo)
    {
      AttributeNameIndex = nameInfo.Index;
    }

    public override byte[] ToBytes()
    {
      using var mem = new MemoryStream();
      using var writer = new ConstantPoolBinaryWriter(mem);
      writer.Write(AttributeNameIndex);
      var bytes = base.ToBytes();
      writer.Write((uint)bytes.LongLength);
      writer.Write(bytes);
      return mem.ToArray();
    }
  }
}
