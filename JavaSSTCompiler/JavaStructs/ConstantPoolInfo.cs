using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.JavaStructs
{
  public struct ConstantPoolInfo
  {
    ConstantPoolTag Tag { get; set; }
    byte[] Info { get; set; }
  }
}
