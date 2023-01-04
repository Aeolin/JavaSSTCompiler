using JavaSST.JavaStructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Compiler.Models
{
  public abstract class AbstractConstantPoolReferenced
  {
    public AbstractConstantPoolReferenced(ConstantPoolInfo info)
    {
      this.ConstantPoolIndex = info.Index;
    }
    
    public int ConstantPoolIndex { get; init; }
  }
}
