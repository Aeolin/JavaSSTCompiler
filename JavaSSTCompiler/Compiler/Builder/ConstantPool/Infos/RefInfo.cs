using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Builder.ConstantPool.Infos
{
  public class RefInfo : AbstractConstantPoolInfo
  {
    [Field(1)]
    public ushort ClassIndex { get; set; }

    [Field(2)]
    public ushort NameAndTypeIndex { get; set; }
    
    public RefInfo(ClassInfo classInfo, NameAndTypeIndex nani, ConstantPoolTag tag, ushort index) : base(tag, index)
    {
      if (tag != ConstantPoolTag.FieldRef && tag != ConstantPoolTag.MethodRef && tag != ConstantPoolTag.InterfaceMethodRef)
        throw new ArgumentException("Invalid tag for RefInfo");

      ClassIndex = classInfo.Index;
      NameAndTypeIndex = nani.Index;
    }
  }
}
