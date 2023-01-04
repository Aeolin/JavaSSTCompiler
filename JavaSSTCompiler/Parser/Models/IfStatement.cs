using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSST.Parser.Models
{
  internal class IfStatement : IStatement
  {
    public Expression Condition { get; set; }
    public IStatement[] ThenStatements { get; set; }
    public IStatement[] ElseStatements { get; set; }
  }
}
