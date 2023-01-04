using JavaSSTCompiler.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaSSTCompiler.Parser.Models
{
  public class Expression : IFactor
  {
    public SimpleExpression Left { get; set; }
    public TokenType? Comparison { get; set; }
    public SimpleExpression Right { get; set; }
  }
}
