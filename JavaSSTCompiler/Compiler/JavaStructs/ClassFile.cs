using JavaSST.Parser.Models;
using JavaSSTCompiler.Compiler.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.JavaStructs
{
    public class ClassFile
  {
    public ClassFile(Class @class)
    {
      
    }

    public UInt32 Magic { get; init; } = 0xBEBAFECA;
    public UInt16 MinorVersion { get; set; } = 0;
    public UInt16 MajorVersion { get; init; } = 51;
    public UInt16 ConstantPoolCount { get; set; }
    public ConstantPoolInfo[] ConstantPool { get; set; }
    public AccessFlags AccessFlags { get; init; } = AccessFlags.Public;
    public UInt16 ThisClass { get; set; }
    public UInt16 SuperClass { get; init; } = 0;
    public UInt16 InterfacesCount { get; init; } = 0;
    //public UInt16[] Interfaces { get; set; }
    public UInt16 FieldsCount { get; set; }
    public FieldInfo[] Fields { get; set; }
    public UInt16 MethodsCount { get; set; }
    public MethodInfo[] Methods { get; set; }
    public UInt16 AttributesCount { get; init; } = 0;
  }
}
