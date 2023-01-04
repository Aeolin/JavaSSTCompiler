using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Parser.Models
{
  public class MethodBody
  {
    public DynamicField[] LocalVariables { get; set; }
    public IStatement[] Statements { get; set; }
  }
}
