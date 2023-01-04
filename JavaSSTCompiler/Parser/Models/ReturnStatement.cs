using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Parser.Models
{
  public class ReturnStatement : IStatement
  {
    public SimpleExpression Expression { get; set; }
  }
}
