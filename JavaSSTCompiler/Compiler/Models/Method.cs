using JavaSST.JavaStructs;
using JavaSSTCompiler.Compiler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.Compiler.Models
{
  public class Method
  {
    public Method(string name, string returnType)
    {
      Name=name;
      ReturnType=returnType;
    }

    public string Name { get; set; }
    public string ReturnType { get; set; }
    public Parameter[] Parameters { get; set; }
  }
}
