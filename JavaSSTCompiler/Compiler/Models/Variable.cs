using JavaSST.JavaStructs;
using JavaSSTCompiler.Compiler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.Compiler.Models
{
  public class Variable : AbstractConstantPoolReferenced
  {
    public string Name { get; set; }
    public string Type { get; set; }
    public bool CanModify { get; set; }

    public Variable(string name, string type, bool canModify, ConstantPoolInfo info) : base(info)
    {
      Name = name;
      Type = type;
      CanModify = canModify;
    }
  }
}
