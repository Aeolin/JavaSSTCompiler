using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Parser.Models
{
  public class WhileStatement : IStatement
  {
    public Expression Condition { get; set; }
    public IStatement[] Statements { get; set; }
  }
}
